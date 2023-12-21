using ExampleLongPollingWithTaskCompletionSource.Api.Models;
using ExampleLongPollingWithTaskCompletionSource.Api.Repository;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TaskTimeoutOptions _taskTimeoutOptions;

        public OrdersController(IOrderRepository orderRepository,
            IOptions<TaskTimeoutOptions> options)
        {
            _taskTimeoutOptions = options.Value;
            _orderRepository = orderRepository;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(_taskTimeoutOptions.TimeoutIntSecounds));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllOrderRequest request)
        {
            var orders = await _orderRepository.GetOrderAsync(request.SerialNumber);

            if (orders == null)
                throw new InvalidOperationException("Unable to search for orders");

            if (orders.Any())
                return Ok(orders);

            var orderLongPolling = new OrderLongPolling($"{request.Id}", $"description {Guid.NewGuid()}", request.SerialNumber, "pending");

            //TIMEOUT CONFIGURADO
            var taskGetOrderTimeout = Task.Delay((int)TimeSpan.FromSeconds(_taskTimeoutOptions.TimeoutIntSecounds).TotalMilliseconds);

            //LONG POLLING
            var taskGetOrdersFromChangeEvents = orderLongPolling.Tcs.Task;

            var tasksCompleted = await Task.WhenAny(taskGetOrderTimeout, taskGetOrdersFromChangeEvents);

            if (tasksCompleted == taskGetOrderTimeout)
                return NoContent();

            orders = await _orderRepository.GetOrderAsync(request.SerialNumber);

            return Ok(orders);
        }
    }
}
