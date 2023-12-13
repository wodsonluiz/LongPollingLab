using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleIAsyncEnumerable
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Handler().GetAwaiter().GetResult(); 
        }

        private static async Task Handler()
        {
            var service = new OrderService();

            var data = new ObservableCollection<Order>();

            var enumerator = service.GetAllOrders();

            await foreach (var item in enumerator.WithCancellation(CancellationToken.None))
            {
                data.Add(item);
                Console.WriteLine(JsonConvert.SerializeObject(item));
            }
        }
    }
}