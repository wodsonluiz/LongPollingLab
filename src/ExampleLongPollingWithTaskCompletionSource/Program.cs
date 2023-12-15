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
            var ordersRequest = Order.GetFakeOrders(10);

            foreach (var request in ordersRequest)
            {
                var simplePolling = new SimplePolling(request?.Id!, request?.SerialNumber!, request?.Description!);

                simplePolling.Push();
            }

            // - BUSCANDO NO BANCO DE DADOS
            // var orderResult = _repository.FindOne(orderRequest);
            // orderResult != null return;
            Task.Delay(300).GetAwaiter().GetResult();


            // - WORKER
            var timeout = Task.Delay(5000, CancellationToken.None);
            // - SEGUE CASO NÃO ENCONTRA PEDIDOS
            // - MONGO TRINGADO EVENTO / CONFIGURANDO TIMEOUT
            var simplePollingResult = HandlerResponseToClient(new OrderEvent { Id = "id_2" }, CancellationToken.None);
            
            
            // - CORRIDA TIMEOUT X LONG POLLING
            var completedTask = Task.WhenAny(simplePollingResult, timeout).GetAwaiter().GetResult();

            if(completedTask != timeout)
            {
                var result = completedTask as Task<SimplePolling>;

                result?.GetAwaiter()
                    .GetResult()
                    ?.Notify();

                // var orderResult = _repository.FindOne(orderRequest);
            }
            else
            {
                Console.WriteLine("Respondeu o timeout");
            }
        }

        static Task<SimplePolling?> HandlerResponseToClient(OrderEvent orderEvent, CancellationToken cancellationToken)
        {
            //Aqui eu estou abrindo uma nova task 
            return Task.Factory.StartNew(() =>
            {
                //Aqui estou abrindo outra task e startei
                Task.Delay(1000, cancellationToken).GetAwaiter().GetResult();

                var simplesPollings = SimplePolling.GetSimplePollings();

                var simplePollingResult = simplesPollings.FirstOrDefault(order => order.Id == orderEvent.Id);

                Console.WriteLine("Mongo notificou");

                return simplePollingResult;

            }, cancellationToken);
        }
    }
}