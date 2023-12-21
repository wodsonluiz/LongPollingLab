using ExampleLongPollingWithTaskCompletionSource.Api.Models;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api
{
    public class ListernerWorker : BackgroundService
    {
        private readonly IMongoService _mongoService;

        public ListernerWorker(IMongoService service) =>
            _mongoService = service;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Handler(stoppingToken);
        }

        private Task Handler(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() => 
            {
                using (var watcher = _mongoService.BuildHandlerListerner<OrderEvent>(stoppingToken))
                {
                    OrderLongPolling orderLongPolling = null;

                    while (watcher.MoveNext())
                    {
                        var orderEvent = watcher.Current.FullDocument;

                        var ordersPolling = OrderLongPolling.GetOrdersLongPollings();

                        if (ordersPolling == null)
                            continue;

                        if (string.IsNullOrEmpty(orderEvent?.SerialNumber))
                            continue;

                        orderLongPolling = ordersPolling.FirstOrDefault(o => o.SerialNumber == orderEvent.SerialNumber);

                        if (orderLongPolling != null)
                            orderLongPolling.Notify();
                    }
                }
            });
        }
    }
}
