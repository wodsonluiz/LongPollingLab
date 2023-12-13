using System;
using System.Collections.Generic;

namespace ExampleTaskCompletionSource
{
    public class Order
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public static IEnumerable<Order> GetFakeOrders(int count)
        {
            var result = new List<Order>();

            for (int i = 0; i < count; i++)
            {
                result.Add(new Order { Id = Guid.NewGuid().ToString(), Description = $"Description test {i}" });
            }

            return result;
        }
    }
}
