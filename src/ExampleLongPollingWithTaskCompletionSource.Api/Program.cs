using ExampleLongPollingWithTaskCompletionSource.Api.Repository;
using ExampleLongPollingWithTaskCompletionSource.Api.Service.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ExampleLongPollingWithTaskCompletionSource.Api
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddMongoService(config);

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

    internal static class ServiceExtensions
    {
        public static OptionsBuilder<T> AddConfig<T>(this IServiceCollection services, string configSectionPath) where T : class
        {
            var builder = services
                .AddOptions<T>()
                .BindConfiguration(configSectionPath)
                .ValidateDataAnnotations();

            return builder;
        }
    }


    internal static class ServiceCollectionExtensions
    {
        public static void AddMongoService(this IServiceCollection services, IConfiguration config)
        {
            services.AddConfig<TaskTimeoutOptions>(TaskTimeoutOptions.ConfigSectionName);
            services.AddSingleton<IMongoProvider>(_ =>
            {
                var conn = config.GetConnectionString("MongoDb");
                return new MongoProvider(conn);
            });
            services.AddSingleton<ITokenManger, TokenManger>();
            services.AddSingleton<IMongoService, MongoService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}