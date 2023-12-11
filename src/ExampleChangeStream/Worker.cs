using ExampleChangeStream.Service;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ExampleChangeStream
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MongoService _mongoProvider;

        public Worker(ILogger<Worker> logger, MongoService mongoProvider)
        {
            _logger = logger;
            _mongoProvider = mongoProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var database = _mongoProvider.GetDatabase("MyCollections");
                var collection = database.GetCollection<Order>("Orders");

                Handler(collection);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void Handler(IMongoCollection<Order> collection)
        {
            //Configurando um pipeline
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Order>>().Match("{ operationType: { $in: ['insert', 'update', 'replace', 'delete'] } }");

            //Abrindo o Change stream
            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };

            var changeStream = collection.Watch(pipeline, options);

            //Manipulando eventos do Change Stream
            using (var enumerator = changeStream.ToEnumerable().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var change = enumerator.Current;

                    Console.WriteLine($"Evento: {change.OperationType}");
                    Console.WriteLine($"Documento: {JsonConvert.SerializeObject(change.FullDocument)}");
                }
            }
        }
    }
}