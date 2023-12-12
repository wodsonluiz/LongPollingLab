using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExampleChangeStream.Service.Mongo
{
    public interface ITokenManger
    {
        void Save(BsonDocument document);
        BsonDocument? Get();
        BsonTimestamp? GetLastUpdatedDocument();
    }

    public class TokenManger : ITokenManger
    {
        private readonly MemoryCache _memoryCache;
        private readonly MongoProvider _provider;

        public TokenManger(MongoProvider provider)
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _provider = provider;
        }

        public BsonDocument? Get() =>
            _memoryCache.Get<BsonDocument?>("TokenMongo");

        public BsonTimestamp? GetLastUpdatedDocument()
        {
            var collection = _provider.GetDatabase("local").GetCollection<BsonDocument>("oplog.rs");

            return GetLastOplogTimestamp(collection);
        }

        public void Save(BsonDocument document) =>
            _memoryCache.Set("TokenMongo", document, TimeSpan.FromMinutes(10));

        private static BsonTimestamp GetLastOplogTimestamp(IMongoCollection<BsonDocument> oplogCollection)
        {
            var sort = Builders<BsonDocument>.Sort.Descending("ts");

            var lastOplogEntry = oplogCollection
                .Find(new BsonDocument())
                .Limit(1)
                .Sort(sort)
                .FirstOrDefault();

            var timestamp = lastOplogEntry?["ts"].AsBsonTimestamp;

            return timestamp ?? new BsonTimestamp(0);
        }
    }
}
