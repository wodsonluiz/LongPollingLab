using Amazon.Runtime.Internal;
using ExampleLongPollingWithTaskCompletionSource.Api.Models;
using ExampleLongPollingWithTaskCompletionSource.Api.Repository;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly IMongoService _mongoService;
        private readonly IOrderRepository _orderRepository;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TaskTimeoutOptions _taskTimeoutOptions;

        public OrdersController(IMongoService mongoService, 
            IOrderRepository orderRepository,
            IOptions<TaskTimeoutOptions> options)
        {
            _taskTimeoutOptions = options.Value;
            _mongoService = mongoService;
            _orderRepository = orderRepository;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(_taskTimeoutOptions.TimeoutIntSecounds));
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get([FromQuery] GetAllOrderRequest request)
        {
            var orders = await _orderRepository.GetOrderAsync(request.SerialNumber);

            if (orders == null)
                throw new InvalidOperationException("Unable to search for orders");

            if (orders.Any())
                return orders;

            var shouldChangeEvents = false;

            _ = new OrderLongPolling($"{request.Id}", $"description {Guid.NewGuid()}", request.SerialNumber, "pending");

            //TIMEOUT CONFIGURADO
            var taskGetOrderTimeout = GetListOrdersEmpty(_cancellationTokenSource.Token);

            //LONG POLLING
            var taskGetOrdersFromChangeEvents = GetOrdersFromChangeStream(_cancellationTokenSource.Token);

            var tasksCompleted = await Task.WhenAny(taskGetOrderTimeout, taskGetOrdersFromChangeEvents);

            if (tasksCompleted != taskGetOrderTimeout)
                shouldChangeEvents = true;

            if (shouldChangeEvents)
                orders = await _orderRepository.GetOrderAsync(request.SerialNumber);

            return orders;
        }

        private Task<IEnumerable<Order>> GetListOrdersEmpty(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                await Task.Delay((int)TimeSpan.FromSeconds(_taskTimeoutOptions.TimeoutIntSecounds).TotalMilliseconds);

                return OrderHelper.GetFakeOrders(0);

            }, stoppingToken);
        }

        private Task GetOrdersFromChangeStream(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var polling = ListenerEvents(stoppingToken);

                polling?.Notify();

                return polling?.Tcs.Task;

            }, stoppingToken);
        }

        private OrderLongPolling? ListenerEvents(CancellationToken stoppingToken)
        {
            using (var watcher = _mongoService.Listener<OrderEvent>(stoppingToken))
            {
                OrderLongPolling orderLongPolling = null;

                while (watcher.MoveNext())
                {
                    var orderEvent = watcher.Current.FullDocument;

                    orderLongPolling = OrderLongPolling.GetOrdersLongPollings()
                        .FirstOrDefault(o => o.SerialNumber == orderEvent.SerialNumber);

                    if (orderLongPolling != null)
                        break;
                }

                return orderLongPolling;
            }
        }
    }
}
