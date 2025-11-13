using Application.Implementations;
using Application.Interfaces;
using DTOs;
using Producer.Services;

namespace Producer
{
    public class Worker : BackgroundService
    {

        private readonly IDataReader<GARecord> _gaReader;
        private readonly IDataReader<PSIRecord> _psiReader;
        private readonly IMessagePublisher _publisher;
        private readonly IAggregatorService _aggregatorService;
        public Worker(IDataReader<GARecord> gaReader,
                      IDataReader<PSIRecord> psiReader,
                      IMessagePublisher publisher,
                      IAggregatorService aggregatorService)
        {
            _gaReader = gaReader;
            _psiReader = psiReader;
            _publisher = publisher;
            _aggregatorService = aggregatorService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Try to initialize RabbitMQ once (with retry)
                const int maxRetries = 10;
                int attempt = 0;
                bool rabbitReady = false;

                while (!rabbitReady && attempt < maxRetries && !stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await _publisher.InitializeAsync();
                        rabbitReady = true;
                        Console.WriteLine("[✓] RabbitMQ connection established.");
                    }
                    catch (Exception ex)
                    {
                        attempt++;
                        Console.WriteLine($"[!] RabbitMQ init failed (attempt {attempt}/{maxRetries}): {ex.Message}");
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                }

                if (!rabbitReady)
                {
                    Console.WriteLine("[✗] Failed to initialize RabbitMQ after retries. Worker will run without publishing.");
                }

                try
                {
                    var gaData = await _gaReader.ReadAsync("mock-data/ga-data.json");
                    var psiData = await _psiReader.ReadAsync("mock-data/psi-data.json");

                    var combined =  _aggregatorService.AggregateAsyn(gaData,psiData);
                   await _publisher.InitializeAsync();

                    if (rabbitReady)
                    {
                        foreach (var record in combined)
                        {
                            await _publisher.PublishAsync(record);
                        }
                    }
                    else
                    {
                        Console.WriteLine("[!] Skipping publish — RabbitMQ not initialized.");
                    }
                    // Wait before next ingestion
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                }
            }
        }
    }
}
