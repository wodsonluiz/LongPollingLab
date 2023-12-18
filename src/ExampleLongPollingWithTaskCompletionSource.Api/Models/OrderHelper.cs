using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Models
{
    public static class OrderHelper
    {
        public static IEnumerable<Order> GetFakeOrders(int count)
        {
            Task.Delay(1000).GetAwaiter().GetResult();

            var orders = new List<Order>();

            if (count <= 0)
                return orders;

            for (int i = 0; i < count; i++)
            {
                orders.Add(new Order { Id = $"id_{i}", Description = $"Description test {i}", SerialNumber = Guid.NewGuid().ToString() });
            }

            return orders;
        }
    }
}
