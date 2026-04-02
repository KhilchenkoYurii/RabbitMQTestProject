using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace WorkerForPriorityQueue
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

            await channel.BasicQosAsync(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Processing message ->:{message} ....");
                Thread.Sleep(1000);
                Console.WriteLine("Finished");

                await channel.BasicAckAsync(e.DeliveryTag, false);
            };

            string consumerTag = await channel.BasicConsumeAsync("my.queue", false, consumer);

            Console.WriteLine("Subscribed to the queue. Waiting for messages");
            Console.ReadKey();

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
