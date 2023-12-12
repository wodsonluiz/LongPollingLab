using MongoDB.Driver;

namespace Service.Mongo
{
    public class MongoProvider
    {
        private MongoClient _client { get; }

        public MongoProvider(string conn) =>
            _client = new MongoClient(conn);

        public IMongoDatabase GetDatabase(string dbName) =>
            _client.GetDatabase(dbName);
    }
}
