using ProductProvider.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text.Json;

namespace ProductProvider.Messages
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly string _hostname = "localhost";
        private readonly string _queueName = "product_updates";

        public Task PublishAsync(string messageType, object messageData)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(messageData);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);

            Console.WriteLine($"[x] Sent {message}");

            return Task.CompletedTask;
        }
    }
}
