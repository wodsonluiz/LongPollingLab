using ExampleChangeStream.Service;

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

                    var mongoProvider = new MongoService(conn);
                    services.AddSingleton(mongoProvider);
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}