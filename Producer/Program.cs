using Application.Implementations;
using Application.Interfaces;
using DTOs;
using Producer.Services;

namespace Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IDataReader<GARecord>, GADataReader>();
                    services.AddSingleton<IDataReader<PSIRecord>, PSIDataReader>();
                    services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
                    services.AddSingleton<IAggregatorService, AggregatorService>();
                    services.AddHostedService<Worker>();
                }) .Build().Run();
            var host = builder.Build();
            host.Run();
        }
    }
}