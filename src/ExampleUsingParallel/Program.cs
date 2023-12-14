using System;
using System.Threading.Tasks;

namespace ExampleUsingParallel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Handler().GetAwaiter().GetResult();
            HandlerWhtiThrow().GetAwaiter().GetResult();
        }

        static async Task Handler()
        {

            await Task.Run(() =>
            {
                Parallel.Invoke(
                    new ParallelOptions { MaxDegreeOfParallelism = 2 },
                    () =>
                    {
                        Console.WriteLine($"Executando a primeira task 1 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                    },
                    () =>
                    {
                        Console.WriteLine($"Executando a primeira task 2 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                    },
                    () =>
                    {
                        Console.WriteLine($"Executando a primeira task 3 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                    },
                    () =>
                    {
                        Console.WriteLine($"Executando a primeira task 4 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                    }
                    );
            });
        }
        static async Task HandlerWhtiThrow()
        {
            try
            {
                await Task.Run(() =>
                {
                    Parallel.Invoke(
                        new ParallelOptions { MaxDegreeOfParallelism = 2 },
                        () =>
                        {
                            Console.WriteLine($"Executando a primeira task 1 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                        },
                        () =>
                        {
                            throw new Exception("Error");
                            Console.WriteLine($"Executando a primeira task 2 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                        },
                        () =>
                        {
                            throw new Exception("Error");
                            Console.WriteLine($"Executando a primeira task 3 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                        },
                        () =>
                        {
                            Console.WriteLine($"Executando a primeira task 4 as {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
                        }
                        );
                });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}