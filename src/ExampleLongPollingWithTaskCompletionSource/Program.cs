using System;
using System.Linq;
using System.Threading;
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
            var simplePollingResult = HandlerResponseToClient(new OrderEvent { Id = "id_2" }, CancellationToken.None);
            var timeout = Task.Delay(5000, CancellationToken.None);
            // - Concorrência com timeout

            var completedTask = Task.WhenAny(simplePollingResult, timeout).GetAwaiter().GetResult();

            if(completedTask != timeout)
            {
                var result = completedTask as Task<SimplePolling>;

                result.GetAwaiter().GetResult()?.Notify();
            }
            else
            {
                Console.WriteLine("Respondeu o timeout");
            }
        }

        static Task<SimplePolling?> HandlerResponseToClient(OrderEvent orderEvent, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                Task.Delay(4000, cancellationToken).GetAwaiter().GetResult();

                var simplesPollings = SimplePolling.GetSimplePollings();

                var simplePollingResult = simplesPollings.FirstOrDefault(order => order.Id == orderEvent.Id);

                Console.WriteLine("Passou pelo handler");

                return simplePollingResult;

            }, cancellationToken);
        }
    }
}