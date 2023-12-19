using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling
{
    public class OrderLongPollingRefactor
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Status { get; set; }
        public TaskCompletionSource<bool> Tcs { get; }

        private static List<OrderLongPollingRefactor> orderLongPollings = new List<OrderLongPollingRefactor>();

        public OrderLongPollingRefactor(string id, string description, string serialNumber, string status)
        {
            Tcs = new TaskCompletionSource<bool>();

            Id = id;
            SerialNumber = serialNumber;
            Description = description;
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

        public static IEnumerable<OrderLongPollingRefactor> GetOrdersLongPollings() => orderLongPollings;
    }
}
