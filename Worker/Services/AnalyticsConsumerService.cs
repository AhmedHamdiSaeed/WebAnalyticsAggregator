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
        private readonly AnalyticsDbContext _db;
        private readonly string _queueName = "analytics.raw.q";
        private IConnection? _connection;
        private IChannel? _channel;

        public AnalyticsConsumerService(AnalyticsDbContext db)
        {
             _db = db;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "user",
                Password = "password",
                VirtualHost = "/"
            };
            var conn = await factory.CreateConnectionAsync();
            using var channel = await conn.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "testQueue",
                                  durable: true,
                                  exclusive: true,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var record = JsonSerializer.Deserialize<CombinedRecord>(message);

                if (record != null)
                {
                    try
                    {
                      //  Save raw data
                        _db.RawData.Add(record);

                    //    Aggregate daily stats
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
                            daily.AvgPerformance = (_db.RawData.Where(r => r.Date == record.Date)
                                                              .Average(r => r.PerformanceScore));
                            daily.LastUpdatedAt = DateTime.UtcNow;
                        }

                        await _db.SaveChangesAsync();

                        await _channel!.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine($"[Consumer] Processed {record.Page} ({record.Date:yyyy-MM-dd})");
                    }
                    catch
                    {
                       await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                }
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
        }
    }
}
