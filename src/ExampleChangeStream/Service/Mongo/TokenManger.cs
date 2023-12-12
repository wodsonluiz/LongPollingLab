using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;

namespace ExampleChangeStream.Service.Mongo
{
    public interface ITokenManger
    {
        void Save(BsonDocument document);
        BsonDocument? Get();
    }

    public class TokenManger: ITokenManger
    {
        private readonly MemoryCache _memoryCache;

        public TokenManger() =>
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public BsonDocument? Get() => 
            _memoryCache.Get<BsonDocument?>("TokenMongo");

        public void Save(BsonDocument document) => 
            _memoryCache.Set("TokenMongo", document, TimeSpan.FromMinutes(10));
    }
}
