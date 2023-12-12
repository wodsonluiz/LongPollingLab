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
        private readonly ChangeStreamOperationType[] _changeStreamOperationTypes;

        public MongoService(MongoProvider provider, ITokenManger tokenManger)
        {
            _provider = provider;
            _tokenManger = tokenManger;

            _changeStreamOperationTypes = new[]
            {
                ChangeStreamOperationType.Update,
                ChangeStreamOperationType.Insert,
                ChangeStreamOperationType.Delete,
                ChangeStreamOperationType.Replace,
            };
        }

        public IEnumerator<ChangeStreamDocument<Order>> Listener(CancellationToken stoppingToken)
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Order>>()
                .Match(change => _changeStreamOperationTypes.Contains(change.OperationType));

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
