using Amazon.Runtime.Internal;
using ExampleLongPollingWithTaskCompletionSource.Api.Models;
using ExampleLongPollingWithTaskCompletionSource.Api.Repository;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        const int CONFIG_TIMEOUT = 600;
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

            if (orders == null)
                throw new InvalidOperationException("Unable to search for orders");

            if (!orders.Any())
            {
                var guid = Guid.NewGuid().ToString();
                var orderPolling = new OrderLongPollingRefactor($"{request.Id}", $"description {guid}", request.SerialNumber, "pending");

                await orderPolling.PushAync(_cancellationTokenSource.Token);

                //TIMEOUT CONFIGURADO
                var taskGetOrderTimeout = GetListOrdersEmpty(_cancellationTokenSource.Token);

                //LONG POLLING
                var taskGetOrdersFromChangeEvents = GetOrdersFromChangeStream(_cancellationTokenSource.Token);

                orders = await Task.WhenAny(taskGetOrdersFromChangeEvents, taskGetOrdersFromChangeEvents).Unwrap();

                return orders;
            }

            return orders;
        }

        private Task<IEnumerable<Order>> GetListOrdersEmpty(CancellationToken stoppingToken)
        {
            try
            {
                return Task.Run(async () =>
                {
                    await Task.Delay((int)TimeSpan.FromSeconds(CONFIG_TIMEOUT).TotalMilliseconds);

                    return OrderHelper.GetFakeOrders(0);

                }, stoppingToken);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task<IEnumerable<Order>> GetOrdersFromChangeStream(CancellationToken stoppingToken)
        {
            try
            {
                var orderLongPolling = await ListenerEvents(stoppingToken);

                if (orderLongPolling != null)
                {
                    var orders = await _orderRepository.GetOrderAsync(orderLongPolling.SerialNumber);
                    return orders;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task<OrderLongPollingRefactor?> ListenerEvents(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    OrderLongPollingRefactor orderLongPolling = null;
                    var watcher = _mongoService.Listener<OrderEvent>(_cancellationTokenSource.Token);

                    while (watcher.MoveNext())
                    {
                        var orderEvent = watcher.Current.FullDocument;

                        orderLongPolling = OrderLongPollingRefactor.GetOrdersLongPollings()
                            .FirstOrDefault(o => o.SerialNumber == orderEvent.SerialNumber);

                        orderLongPolling.Tcs.SetResult(true);

                        break;
                    }

                    return orderLongPolling;

                }, stoppingToken);

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
