using System.Linq;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // - CHEGANDO REQUEST DO CLIENTE
            var ordersRequest = Order.GetFakeOrders(5);

            foreach (var request in ordersRequest)
            {
                var simplePolling = new SimplePolling(request?.Id!, request?.SerialNumber!);

                simplePolling.Push();
            }

            // - BUSCANDO NO BANCO DE DADOS
            Task.Delay(300).GetAwaiter().GetResult();


            // - Mongo trigou o evento
            Handler(new OrderEvent { Id = "id_2" });
        }

        static void Handler(OrderEvent orderEvent)
        {
            var simplesPollings = SimplePolling.GetSimplePollings();

            var simplePollingResult = simplesPollings.FirstOrDefault(order => order.Id == orderEvent.Id);

            if (simplePollingResult == null)
                return;

            simplePollingResult.Notify();
        }
    }
}