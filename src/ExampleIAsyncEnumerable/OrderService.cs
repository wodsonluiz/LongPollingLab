using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleIAsyncEnumerable
{
    public class OrderService
    {
        /// <summary>
        /// GetAllOrders Lista todos os pedidos
        /// Aqui criamos um fluxo de dados para ir recuperando cada pedido de acordo com a disponibilidade.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Order> GetAllOrders([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);
            yield return new Order() { Id = 1, Description = "Description 1" };
            await Task.Delay(500, cancellationToken);
            yield return new Order() { Id = 2, Description = "Description 2" };
            await Task.Delay(500, cancellationToken);
            yield return new Order() { Id = 3, Description = "Description 3" };
            await Task.Delay(500, cancellationToken);
            yield return new Order() { Id = 4, Description = "Description 4" };
            await Task.Delay(500, cancellationToken);
            yield return new Order() { Id = 5, Description = "Description 5" };
            await Task.Delay(500, cancellationToken);
            yield return new Order() { Id = 6, Description = "Description 6" };
        }
    }
}
