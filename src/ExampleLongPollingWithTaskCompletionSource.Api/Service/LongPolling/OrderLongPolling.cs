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

        private static AsyncLocal<List<OrderLongPolling>> orderLongPollingsAsync = new();
        public readonly TaskCompletionSource<bool> Tcs;

        public OrderLongPolling(string id, string description, string serialNumber, string status)
        {
            Id = id;
            SerialNumber = serialNumber;
            Description = description;
            Status = status;

            Tcs = new TaskCompletionSource<bool>();

            lock (orderLongPollingsAsync!)
            {
                if (orderLongPollingsAsync.Value == null)
                    orderLongPollingsAsync.Value = new List<OrderLongPolling>();

                orderLongPollingsAsync?.Value.Add(this);
            }
        }

        public void Notify()
        {
            Tcs?.SetResult(true);
            orderLongPollingsAsync?.Value?.Remove(this);
        }

        public static IEnumerable<OrderLongPolling> GetOrdersLongPollings() => orderLongPollingsAsync.Value!;
    }
}
