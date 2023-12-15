using System.Collections.Generic;
using System.Threading.Tasks;

namespace Name
{
    public class SimplePolling
    {
        public static List<SimplePolling> SimplePollings = new List<SimplePolling>();
        private readonly TaskCompletionSource<bool> _cts;
        public SimplePolling() =>
            _cts = new TaskCompletionSource<bool>();

        public bool Notify()
        {
            _cts.SetResult(true);
            return true;
        }
    }
}