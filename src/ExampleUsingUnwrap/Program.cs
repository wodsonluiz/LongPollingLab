using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleUsingUnwrap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            var task = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(500);

                return "Qualquer texto";
            });

            var task2 = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(500);

                return "Qualquer texto";
            }).Unwrap();

            var result = task.GetAwaiter().GetResult();
            var result2 = task2.GetAwaiter().GetResult();

            Console.WriteLine(result);
            Console.WriteLine(result2);
            Console.WriteLine("Completed");

            Console.WriteLine("****** Passing a value to Task.Factory.StartNew");

            var orders = Order.GetFakeOrders(10);

            Task.Factory.StartNew((state) =>
            {
                var orders = state as IEnumerable<Order>;

                foreach (var order in orders) 
                {
                    Console.WriteLine(JsonConvert.SerializeObject(order));
                }

            }, orders).GetAwaiter().GetResult();
        }
    }
}