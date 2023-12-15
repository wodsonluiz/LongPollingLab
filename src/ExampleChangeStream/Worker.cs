using Newtonsoft.Json;
using Service.Models;
using Service.Mongo;

namespace ExampleChangeStream
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMongoService _service;
        private readonly ITokenManger _tokenManger;

        public Worker(ILogger<Worker> logger, IMongoService service, ITokenManger tokenManger)
        {
            _logger = logger;
            _service = service;
            _tokenManger = tokenManger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Console.Out.WriteLineAsync($"Worker running at: {DateTimeOffset.Now}");
                WatchEventMongo(stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void WatchEventMongo(CancellationToken stoppingToken)
        {
            try
            {
                var watcher = _service.Listener<Order>(stoppingToken);

                while (watcher.MoveNext())
                {
                    var change = watcher.Current;
                    var result = JsonConvert.SerializeObject(change.FullDocument);

                    Console.WriteLine("******* OPERATION TYPE ********");
                    Console.WriteLine(change.OperationType);
                    Console.WriteLine("******* DOCUMENTO ********");
                    Console.WriteLine(result);

                    _tokenManger.Save(change.ResumeToken);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR {ex.Message}");
            }
        }
    }
}