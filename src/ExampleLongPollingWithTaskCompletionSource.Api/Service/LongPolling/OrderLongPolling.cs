using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling
{
    public class OrderLongPolling
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Status { get; set; }

        private static List<OrderLongPolling> orderLongPollings = new List<OrderLongPolling>();
        private readonly TaskCompletionSource<bool> cts;

        public OrderLongPolling(string id, string description, string serialNumber, string status)
        {
            Id = id;
            SerialNumber = serialNumber;
            Description = description;

            cts = new TaskCompletionSource<bool>(TaskContinuationOptions.RunContinuationsAsynchronously);
        }

        public Task PushAync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() =>
            {
                lock (orderLongPollings!)
                {
                    orderLongPollings.Add(this);
                }
            }, stoppingToken);
        }

        public Task NotifyAync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() =>
            {
                lock (cts)
                {
                    this.cts.SetResult(true);
                }
            }, stoppingToken);
        }

        public Task RemoveAync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() =>
            {
                lock (orderLongPollings!)
                {
                    orderLongPollings.Remove(this);
                }
            }, stoppingToken);
        }


        public static IEnumerable<OrderLongPolling> GetOrdersLongPollings() => orderLongPollings;
    }
}
