using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleTaskCompletionSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var orders = Handler();

            Console.WriteLine("****************** Orders **********************");
            Console.WriteLine(JsonConvert.SerializeObject(orders));
        }

        private static Task<IEnumerable<Order>> Handler()
        {
            var tcs = new TaskCompletionSource<IEnumerable<Order>>(TaskContinuationOptions.RunContinuationsAsynchronously);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (int i = 0; i < 5; i++)
                {
                    var orders = Order.GetFakeOrders(10);

                    tcs.SetResult(orders);
                }
            });

            return tcs.Task;
        }
    }
}