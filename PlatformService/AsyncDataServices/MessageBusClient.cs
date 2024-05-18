using PlatformService.DTOs;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to MEssage Bus");

            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO)
        {
            var message = JsonSerializer.Serialize(platformPublishedDTO);

            if(_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection is open and sending message");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is NOT open and NOT sending any message");
            }
        }


        private void RabbitMQ_ConnectionShutdown(object sender, EventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection Shutdown");
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                    exchange: "trigger",
                    routingKey: "",
                    basicProperties: null,
                    body: body
                    );
            Console.WriteLine($"We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message bus Disposed");

            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}
