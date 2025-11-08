using Producer.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Producer.Services
{
    public class RabbitMqPublisher: IMessagePublisher, IDisposable
    {
        private IConnection? _connection;
        private IChannel _channel;
        private readonly string _hostname;
        private const string QueueName = "analytics.raw.q";

        public RabbitMqPublisher(string hostname = "localhost")
        {
            _hostname = hostname; // store hostname, don't create connection here
        }

        /// <summary>
        /// Initialize the RabbitMQ connection and channel.
        /// Call this before publishing messages.
        /// </summary>
        public async Task InitializeAsync()
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
            Console.WriteLine("[x] RabbitMQ initialized.");
        }

        /// <summary>
        /// Publish a record to the queue
        /// </summary>
        public async Task PublishAsync(CombinedRecord record)
        {
            if (_channel == null) throw new InvalidOperationException("Publisher not initialized.");

            var message = JsonSerializer.Serialize(record);
            var body = Encoding.UTF8.GetBytes(message);

            // wrap synchronous BasicPublish in Task.Run
             await _channel.BasicPublishAsync(exchange: "", routingKey: "testQueue", body: body);
             Console.WriteLine($"[x] Published: {record.Page} ({record.Date})");
            
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
