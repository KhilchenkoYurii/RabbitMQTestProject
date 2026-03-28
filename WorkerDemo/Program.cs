using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PushPullDemo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter worker name:");
            var workerName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            var connection = await factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender,e) =>{
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Worker name: {workerName} Message:{message}");
            };

            string consumerTag = await channel.BasicConsumeAsync("my.queue1", true, consumer);

            Console.WriteLine("Waiting for messages. Press any key to exit.Main");
            Console.ReadKey();

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}