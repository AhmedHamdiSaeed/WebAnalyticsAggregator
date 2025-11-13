using Consumer.Data;
using Consumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Worker.Services
{
    public class AnalyticsConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _queueName = "analytics.raw.q";
        private IConnection? _connection;
        private IChannel? _channel;
        private ILogger<AnalyticsConsumerService> _logger;

        public AnalyticsConsumerService(IServiceScopeFactory scopeFactory, ILogger<AnalyticsConsumerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int maxRetries = 10;
            int attempt = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // --- Try to connect ---
                    var factory = new ConnectionFactory
                    {
                        HostName = "rabbitmq",
                        UserName = "user",
                        Password = "password",
                        VirtualHost = "/"
                    };

                    Console.WriteLine("[*] Connecting to RabbitMQ...");
                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();

                    await _channel.QueueDeclareAsync(
                        queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    Console.WriteLine("[✓] Connected and queue declared.");

                    var consumer = new AsyncEventingBasicConsumer(_channel);
                
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        try
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var _db = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            _logger.LogWarning("message data =====================> ", message);
                            var record = JsonSerializer.Deserialize<CombinedRecord>(message);
                            _logger.LogWarning("recode data ===> ",record);
                            if (record == null)
                            {
                                await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                                return;
                            }
                            
                            // --- Save to DB ---
                            _db.RawData.Add(record);

                            var daily = _db.DailyStats.FirstOrDefault(d => d.Date == record.Date);
                            if (daily == null)
                            {
                                daily = new DailyStats
                                {
                                    Date = record.Date,
                                    TotalUsers = record.Users,
                                    TotalSessions = record.Sessions,
                                    TotalViews = record.Views,
                                    AvgPerformance = record.PerformanceScore,
                                    LastUpdatedAt = DateTime.UtcNow
                                };
                                _db.DailyStats.Add(daily);
                            }
                            else
                            {
                                daily.TotalUsers += record.Users;
                                daily.TotalSessions += record.Sessions;
                                daily.TotalViews += record.Views;
                                daily.AvgPerformance = _db.RawData
                                    .Where(r => r.Date == record.Date)
                                    .Average(r => r.PerformanceScore);
                                daily.LastUpdatedAt = DateTime.UtcNow;
                            }

                            await _db.SaveChangesAsync(stoppingToken);
                            await _channel.BasicAckAsync(ea.DeliveryTag, false);

                            Console.WriteLine($"[Consumer] Processed {record.Page} ({record.Date:yyyy-MM-dd})");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[!] Error processing message: {ex.Message}");
                            await _channel!.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                        }
                    };

                    await _channel.BasicConsumeAsync(
                        queue: _queueName,
                        autoAck: false,
                        consumer: consumer
                    );
                    Console.WriteLine("[*] Waiting for messages...");
                    // Keep the connection alive
                    while (!stoppingToken.IsCancellationRequested)
                        await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"[!] RabbitMQ connection failed (attempt {attempt}): {ex.Message}");
                    if (attempt >= maxRetries)
                    {
                        Console.WriteLine("[✗] Max retries reached. Waiting 1 minute before trying again...");
                        attempt = 0;
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[*] Stopping consumer...");
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            return base.StopAsync(cancellationToken);
        }
    }
}
