using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleTaskCompletionSource
{
    public class Order
    {
        public event EventHandler<EventArgs>? DataRead;
        public string? Id { get; set; }
        public string? Description { get; set; }

        public static IEnumerable<Order> GetFakeOrders(int count)
        {
            var result = new List<Order>();

            for (int i = 0; i < count; i++)
            {
                result.Add(new Order { Id = Guid.NewGuid().ToString(), Description = $"Description test {i}" });
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
