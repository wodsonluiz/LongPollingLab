using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleTaskCompletionSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var order = Order.GetFakeOrders(1).FirstOrDefault();
            Handler(order).GetAwaiter().GetResult();
        }

        private static async Task Handler(Order order)
        {
            var tcs = new TaskCompletionSource<bool>();

            order.DataRead += (sender, e) => 
            {
                Console.WriteLine("Chamando o processo de pagamento");
                tcs.TrySetResult(true);
            };

            order.Pay();

            await tcs.Task;

            System.Console.WriteLine("Processo de pagamento concluido");
        }
    }
}