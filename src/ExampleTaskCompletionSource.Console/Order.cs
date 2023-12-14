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
    
        public void Pay()
        {
            Task.Factory.StartNew(() => 
            {
                Console.WriteLine("Iniciando o pagamento");
                Task.Delay(100000);
                PayConfirmed();
            });
        }

        protected virtual void PayConfirmed()
        {
            DataRead?.Invoke(this, EventArgs.Empty);
        }
    }
}
