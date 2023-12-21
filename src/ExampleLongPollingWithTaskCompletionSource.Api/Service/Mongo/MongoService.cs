﻿using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo
{
    public interface IMongoService
    {
        IEnumerator<ChangeStreamDocument<T>> BuildHandlerListerner<T>();
    }

    public class MongoService : IMongoService
    {
        private readonly IMongoProvider _provider;
        private readonly ChangeStreamOperationType[] _changeStreamOperationTypes;

        public MongoService(IMongoProvider provider)
        {
            _provider = provider;

            _changeStreamOperationTypes = new[]
            {
                ChangeStreamOperationType.Update,
                ChangeStreamOperationType.Insert,
            };
        }

        public IEnumerator<ChangeStreamDocument<T>> BuildHandlerListerner<T>()
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<T>>()
            .Match(change => _changeStreamOperationTypes.Contains(change.OperationType));

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
            };

            var collection = _provider.GetDatabase("MyCollections").GetCollection<T>("Orders");

            var changeStream = collection.Watch(pipeline, options)
                .ToEnumerable()
                .GetEnumerator();

            return changeStream;
        }
    }
}
