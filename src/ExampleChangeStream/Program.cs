using Service.Mongo;

namespace ExampleChangeStream
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var conn = "mongodb+srv://admin:PxyxsbtC9EW5c067@clusterdev.3dcvmij.mongodb.net/?retryWrites=true&w=majority";
                    services.AddSingleton(new MongoProvider(conn));
                    services.AddSingleton<IMongoService, MongoService>();
                    services.AddSingleton<ITokenManger, TokenManger>();

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}