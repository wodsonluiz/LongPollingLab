using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource
{
    public class Order
    {
        public event EventHandler<EventArgs>? DataRead;
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }

        public static IEnumerable<Order> GetFakeOrders(int count)
        {
            Task.Delay(1000).GetAwaiter().GetResult();

            var orders = new List<Order>();

            for (int i = 0; i < count; i++)
            {
                orders.Add(new Order { Id = $"id_{i}", Description = $"Description test {i}", SerialNumber = Guid.NewGuid().ToString() });
            }

            return orders;
        }
    
        public void Pay(string pagamento)
        {
            Task.Factory.StartNew(async () => 
            {
                Console.WriteLine($"Iniciando o pagamento - {pagamento}");
                await Task.Delay(5000);
                DataRead?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
