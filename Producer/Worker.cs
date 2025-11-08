using Producer.Models;
using Producer.Services;

namespace Producer
{
    public class Worker : BackgroundService
    {

        private readonly IDataReader<GARecord> _gaReader;
        private readonly IDataReader<PSIRecord> _psiReader;
        private readonly IMessagePublisher _publisher;

        public Worker(IDataReader<GARecord> gaReader,
                      IDataReader<PSIRecord> psiReader,
                      IMessagePublisher publisher)
        {
            _gaReader = gaReader;
            _psiReader = psiReader;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var gaData = await _gaReader.ReadAsync("mock-data/ga-data.json");
                    var psiData = await _psiReader.ReadAsync("mock-data/psi-data.json");

                    var combined = from ga in gaData
                                   join psi in psiData
                                   on new { ga.Date, ga.Page } equals new { psi.Date, psi.Page }
                                   select new CombinedRecord(
                                       ga.Date, ga.Page,
                                       ga.Users, ga.Sessions, ga.Views,
                                       psi.PerformanceScore, psi.LCP_ms);

                    foreach (var record in combined)
                    {
                        await _publisher.PublishAsync(record);
                    }

                    // Wait before next ingestion
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Error: {ex.Message}");
                }
            }
        }
    }
}
