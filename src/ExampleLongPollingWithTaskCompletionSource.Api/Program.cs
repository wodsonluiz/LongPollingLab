using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExampleLongPollingWithTaskCompletionSource.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IMongoProvider>(_ =>
            {
                var conn = "mongodb+srv://admin:PxyxsbtC9EW5c067@clusterdev.3dcvmij.mongodb.net/?retryWrites=true&w=majority";
                return new MongoProvider(conn);
            });
            builder.Services.AddSingleton<ITokenManger,  TokenManger>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}