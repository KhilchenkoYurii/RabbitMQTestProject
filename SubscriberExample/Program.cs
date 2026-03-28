using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PublisherExample
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Enter queue name:");
            var queuName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            var connection = await factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, e) => {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Subscriber [{queuName}] Message:{message}");
            };

            string consumerTag = await channel.BasicConsumeAsync(queuName ?? "my.queue1", true, consumer);

            Console.WriteLine($"Subscribed to {queuName}. Press any key to exit.Main");
            Console.ReadKey();

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}