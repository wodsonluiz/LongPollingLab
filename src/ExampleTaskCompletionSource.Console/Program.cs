﻿using Newtonsoft.Json;
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
                Console.WriteLine("Solicitando ao TCS para finalizar a task");
                tcs.TrySetResult(true);
            };

            order.Pay("1");

            await Task.Delay(5000);

            await tcs.Task;

        }
    }
}