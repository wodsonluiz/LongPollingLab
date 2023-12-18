using ExampleLongPollingWithTaskCompletionSource.Api.Models;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrderAsync(string serialNumber);
    }

    public class OrderRepository: IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;

        public OrderRepository(IMongoProvider mongoProvider)
        {
            _collection = mongoProvider.GetDatabase("MyCollections").GetCollection<Order>("Orders");
        }

        public async Task<IEnumerable<Order>> GetOrderAsync(string serialNumber)
        {
            try
            {
                var filtro = Builders<Order>.Filter.Eq(o => o.SerialNumber, serialNumber);

                var orders = await _collection.FindAsync(filtro);

                return orders.ToList();
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
