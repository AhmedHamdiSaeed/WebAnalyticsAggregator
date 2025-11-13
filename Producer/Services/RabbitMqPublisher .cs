using DTOs;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Producer.Services
{
    public class RabbitMqPublisher: IMessagePublisher, IDisposable
    {
        private IConnection? _connection;
        private IChannel _channel;
        private const string QueueName = "analytics.raw.q";



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
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: QueueName,
                                  durable: true,
                                  exclusive: false,
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
             await _channel.BasicPublishAsync(exchange: "", routingKey: QueueName, body: body);
             Console.WriteLine($"[✓] Published: {record.Page} ({record.Date})");
            
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
