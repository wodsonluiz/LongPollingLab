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
            var result = new List<Order>();

            for (int i = 0; i < count; i++)
            {
                result.Add(new Order { Id = $"id_{i}", Description = $"Description test {i}", SerialNumber = Guid.NewGuid().ToString() });
            }

            return result;
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
