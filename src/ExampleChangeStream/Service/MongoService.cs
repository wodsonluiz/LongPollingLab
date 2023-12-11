using MongoDB.Driver;

namespace ExampleChangeStream.Service
{
    public class MongoService
    {
        private MongoClient _client { get; }

        public MongoService(string conn)
        {
            _client = new MongoClient(conn);
        }

        public IEnumerator<ChangeStreamDocument<Order>> ListenerEvents(CancellationToken stoppingToken)
        {
            //Configurando um pipeline

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Order>>()
                .Match("{ operationType: { $in: ['insert', 'update', 'replace', 'delete'] } }")
                .Match("{ 'fullDocument.Status': 'closed' }");

            //Abrindo o Change stream
            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
            };

            var collection = GetDatabase("MyCollections").GetCollection<Order>("Orders");

            var changeStream = collection.Watch(pipeline, options, stoppingToken)
                .ToEnumerable()
                .GetEnumerator();

            return changeStream;
        }


        private IMongoDatabase GetDatabase(string dbName) =>
            _client.GetDatabase(dbName);
    }
}
