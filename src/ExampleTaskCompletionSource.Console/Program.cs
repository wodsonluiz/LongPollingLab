using System;
using System.Threading.Tasks;

namespace ExampleTaskCompletionSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tcs = new TaskCompletionSource<int>();

            Task.Run(async () =>
            {
                await Task.Delay(20000);
                tcs.TrySetResult(42);
            });

            var result = tcs.Task.GetAwaiter().GetResult();
            Console.WriteLine($"Resultado da tarefa {result}");
        }
    }
}