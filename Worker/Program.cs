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
                // Add EF Core DbContext
                services.AddDbContext<AnalyticsDbContext>(options =>
                    options.UseSqlServer("Server=db;Database=AnalyticsDb;User Id=sa;Password=password;"));

                // Add your consumer worker
                services.AddHostedService<AnalyticsConsumerService>();
            })
            .Build();

            await host.RunAsync();
        }
    }
}