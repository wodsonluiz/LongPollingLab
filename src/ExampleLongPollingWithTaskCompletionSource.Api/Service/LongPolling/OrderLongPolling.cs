using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public void Push()
        {
            lock (orderLongPollings!)
            {
                orderLongPollings.Add(this);
            }
        }

        public void Notify()
        {
            lock (orderLongPollings!)
            {
                cts.SetResult(true);
            }
        }

        public void Remove()
        {
            lock (orderLongPollings!)
            {
                orderLongPollings.Remove(this);
            }
        }


        public static IEnumerable<OrderLongPolling> GetOrdersLongPollings() => orderLongPollings;
    }
}
