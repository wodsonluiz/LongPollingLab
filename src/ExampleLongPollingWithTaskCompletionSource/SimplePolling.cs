using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource
{
    public class SimplePolling
    {
        public string Id { get; }
        public string SerialNumber { get; }
        public string Description { get; set; }

        private static List<SimplePolling> simplePollings = new List<SimplePolling>();
        private readonly TaskCompletionSource<bool> cts;

        public SimplePolling(string id, string serialNumber, string description)
        {
            Id = id;
            SerialNumber = serialNumber;
            Description = description;
            cts = new TaskCompletionSource<bool>();
        }

        public void Push()
        {
            lock (simplePollings!)
            {
                simplePollings.Add(this);
            }
        }


        public void Notify()
        {
            lock (simplePollings!)
            {
                Console.WriteLine($"Evento respondido pro cliente o objeto {JsonConvert.SerializeObject(this)}");
                cts.SetResult(true);
            }
        }

        public static IEnumerable<SimplePolling> GetSimplePollings() => simplePollings;
    }
}
