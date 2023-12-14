using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace ExampleConcurrentBag
{
    public class Order
    {
        public int Id { get; set; }
        public string? Description { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            ConcurrentBag<Task<Order>> bag = new ConcurrentBag<Task<Order>>();

            for (int i = 0; i < 5; i++)
            {
                var numberTask = i;
                var taskWithOrder = Task.Run(() => new Order { Id = numberTask, Description = $"Description {numberTask}" });
                bag.Add(taskWithOrder);
            }
            Console.WriteLine($"Quantidade no bag: {bag.Count}");
            Console.WriteLine("************* Inicio - Retirando do bag ******************");

            while (!bag.IsEmpty)
            {
                Task<Order> order;
                if (bag.TryTake(out order))
                {
                    var resultOrder = order.GetAwaiter().GetResult();
                    Console.WriteLine(JsonConvert.SerializeObject(resultOrder));
                }
            }
            Console.WriteLine("************* Fim - Retirando do bag ******************");
            Console.WriteLine($"Quantidade no bag: {bag.Count}");
        }
    }
}