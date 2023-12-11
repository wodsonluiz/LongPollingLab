using ExampleChangeStream.Service;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ExampleChangeStream
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MongoService _mongoService;

        public Worker(ILogger<Worker> logger, MongoService mongoProvider)
        {
            _logger = logger;
            _mongoService = mongoProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                WatchEventMongo(stoppingToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void WatchEventMongo(CancellationToken stoppingToken)
        {
            var watcher = _mongoService.ListenerEvents(stoppingToken);

            using (watcher) 
            {
                while (watcher.MoveNext())
                {
                    var change = watcher.Current;
                    var result = JsonConvert.SerializeObject(change.FullDocument);

                    Console.WriteLine(change.OperationType);
                    Console.WriteLine("******* DOCUMENTO ********");
                    Console.WriteLine(result);
                }
            }
        }
    }
}