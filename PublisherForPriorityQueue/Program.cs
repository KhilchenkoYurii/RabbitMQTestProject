using RabbitMQ.Client;
using System.Text;

namespace PublisherForPriorityQueue
{
    public class Program
    {

        public static async Task Main(string[] args)
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            var connection = await factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                "ex.fanout",
                "fanout",
                true,
                false,
                null);

            var queueArguments = new Dictionary<string, object?>()
            {
                { "x-max-priority", 2 }
            };

            await channel.QueueDeclareAsync(
                "my.queue",
                true,
                false,
                false,
                queueArguments);

            await channel.QueueBindAsync("my.queue", "ex.fanout", "");

            Console.WriteLine("Publisher is ready. Press any key to send messages");
            Console.ReadKey();

            SendMessage(channel, 1);
            SendMessage(channel, 1);
            SendMessage(channel, 1);

            SendMessage(channel, 2);
            SendMessage(channel, 2);


            Console.WriteLine("Waiting for messages. Press any key to exit.Publisher");
            Console.ReadKey();

            await channel.CloseAsync();
            await connection.CloseAsync();
        }

        private static async void SendMessage(IChannel channel, byte priority)
        {
            var basicProperty = new BasicProperties()
            {
                Priority = priority,
            };

            var message = $"Message with priority: {priority}";

            await channel.BasicPublishAsync("ex.fanout", "", false, basicProperty, Encoding.UTF8.GetBytes(message));

            Console.WriteLine($"Sent: {message}");
        }
    }
}
