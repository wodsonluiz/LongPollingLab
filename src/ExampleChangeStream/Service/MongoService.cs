using MongoDB.Driver;

namespace ExampleChangeStream.Service
{
    public class MongoService
    {
        public MongoClient Client { get; }

        public MongoService(string conn)
        {
            Client = new MongoClient(conn);
        }

        public IMongoDatabase GetDatabase(string dbName)
        {
            return Client.GetDatabase(dbName);
        }
    }
}
