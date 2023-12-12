using MongoDB.Bson;
using MongoDB.Driver;

namespace ExampleChangeStream.Service.Mongo
{
    public interface IMongoService
    {
        IEnumerator<ChangeStreamDocument<Order>> Listener(CancellationToken stoppingToken);
    }

    public class MongoService: IMongoService
    {
        private readonly MongoProvider _provider;
        private readonly ITokenManger _tokenManger;

        public MongoService(MongoProvider provider, ITokenManger tokenManger)
        {
            _provider = provider;
            _tokenManger = tokenManger;
        }

        public IEnumerator<ChangeStreamDocument<Order>> Listener(CancellationToken stoppingToken)
        {
            //Configurando um pipeline
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Order>>()
                //.Match("{ operationType: { $in: ['insert', 'update', 'replace', 'delete'] } }")
                .Match(change => change.OperationType == ChangeStreamOperationType.Update || change.OperationType == ChangeStreamOperationType.Insert)
                .Match(change => change.FullDocument.Status == "pending");

            //Abrindo o Change stream
            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
                ResumeAfter = _tokenManger.Get()
            };

            var collection = _provider.GetDatabase("MyCollections").GetCollection<Order>("Orders");

            var changeStream = collection.Watch(pipeline, options, stoppingToken)
                .ToEnumerable()
                .GetEnumerator();

            return changeStream;
        }
    }
}
