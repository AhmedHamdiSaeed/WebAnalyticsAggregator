using Consumer.Data;
using Microsoft.EntityFrameworkCore;
using Worker.Services;

namespace worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {          
            var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Get IConfiguration from context
                var configuration = context.Configuration;

                // Add EF Core DbContext using connection string from configuration
                services.AddDbContext<AnalyticsDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                // Add your worker service
                services.AddHostedService<AnalyticsConsumerService>();
            })
            .Build();

            await host.RunAsync();
        }
    }
}