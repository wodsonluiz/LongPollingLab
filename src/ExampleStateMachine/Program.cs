using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ExampleStateMachine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var order = Run().GetAwaiter().GetResult();

            Console.WriteLine(JsonConvert.SerializeObject(order));
        }

        private static Task<Order> Run()
        {
            return Compile();
        }

        private static Task<Order> Compile()
        {
            return Load();
        }

        private static Task<Order> Load()
        {
            return Task.Run(() => 
            {
                return new Order { Id = 1, Description = "Description 1" };
            });
        }
    }
}