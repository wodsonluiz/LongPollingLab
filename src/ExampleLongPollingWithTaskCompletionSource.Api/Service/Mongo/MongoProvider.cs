using MongoDB.Driver;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo
{
    public interface IMongoProvider
    {
        IMongoDatabase GetDatabase(string dbName);
    }

    public class MongoProvider: IMongoProvider
    {
        private MongoClient _client { get; }

        public MongoProvider(string conn) =>
            _client = new MongoClient(conn);

        public IMongoDatabase GetDatabase(string dbName) =>
            _client.GetDatabase(dbName);
    }
}
