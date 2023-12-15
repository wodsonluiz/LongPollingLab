using MongoDB.Bson;
using MongoDB.Driver;

namespace Service.Mongo
{
    public interface IMongoService
    {
        IEnumerator<ChangeStreamDocument<T>> Listener<T>(CancellationToken stoppingToken);
    }

    public class MongoService : IMongoService
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

        public IEnumerator<ChangeStreamDocument<T>> Listener<T>(CancellationToken stoppingToken)
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<T>>()
            .Match(change => _changeStreamOperationTypes.Contains(change.OperationType));

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
            };

            //var lastDocumentInOpLog = _tokenManger.GetLastUpdatedDocument();

            //if (lastDocumentInOpLog != null)
            //    options.StartAtOperationTime = lastDocumentInOpLog;

            var collection = _provider.GetDatabase("MyCollections").GetCollection<T>("Orders");

            var changeStream = collection.Watch(pipeline, options, stoppingToken)
                .ToEnumerable()
                .GetEnumerator();

            return changeStream;
        }
    }
}
