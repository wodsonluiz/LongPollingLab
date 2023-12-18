using ExampleLongPollingWithTaskCompletionSource.Api.Models;
using ExampleLongPollingWithTaskCompletionSource.Api.Repository;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        const int CONFIG_TIMEOUT = 120;
        private readonly IMongoService _mongoService;
        private readonly IOrderRepository _orderRepository;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public OrdersController(IMongoService mongoService, IOrderRepository orderRepository)
        {
            _mongoService = mongoService;
            _orderRepository = orderRepository;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(CONFIG_TIMEOUT));
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get([FromQuery] GetAllOrderRequest request)
        {
            var orders = await _orderRepository.GetOrderAsync(request.SerialNumber);

            if(orders.Count() == 0)
            {
                var guid = Guid.NewGuid().ToString();
                var orderPolling = new OrderLongPolling($"{request.Id}", $"description {guid}", request.SerialNumber, "pending");
                orderPolling.Push();

                var taskGetOrdersFromChangeEvents = GetOrdersFromChangeStream();
                var taskGetOrderTimeout = GetListOrdersEmpty();

                var result = await Task.WhenAny(taskGetOrdersFromChangeEvents, taskGetOrderTimeout);

                if (result == taskGetOrderTimeout)
                    orderPolling.Remove();

                var response = await result;

                return response;
            }

            return orders;
        }

        private Task<IEnumerable<Order>> GetListOrdersEmpty()
        {
            return Task.Run(async () =>
            {
                await Task.Delay((int)TimeSpan.FromSeconds(CONFIG_TIMEOUT).TotalMilliseconds);

                return OrderHelper.GetFakeOrders(0);

            }, _cancellationTokenSource.Token);
        }

        private async Task<IEnumerable<Order>> GetOrdersFromChangeStream()
        {
            var orderLongPolling = await ListenerEvents();

            if (orderLongPolling != null) 
            {
                orderLongPolling.Notify();
                var orders = await _orderRepository.GetOrderAsync(orderLongPolling.SerialNumber);
                return orders;
            }

            return null;
        }

        private async Task<OrderLongPolling?> ListenerEvents()
        {
            OrderLongPolling orderLongPolling = null;

            await Task.Factory.StartNew(() =>
            {
                var watcher = _mongoService.Listener<OrderEvent>(_cancellationTokenSource.Token);

                while (watcher.MoveNext())
                {
                    var orderEvent = watcher.Current.FullDocument;

                    orderLongPolling = OrderLongPolling.GetOrdersLongPollings()
                        .FirstOrDefault(o => o.SerialNumber == orderEvent.SerialNumber);

                    if (orderLongPolling != null)
                    {
                        break;
                    }
                }

            }, _cancellationTokenSource.Token);

            return orderLongPolling;
        }
    }
}
