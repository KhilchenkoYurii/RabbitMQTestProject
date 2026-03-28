using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PushPullDemo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            var connection = await factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            //await ReadMessagesWithPushModel(channel);
            await ReadMessagesWithPullModel(channel);

            Console.WriteLine("Waiting for messages. Press any key to exit.Main");
            Console.ReadKey();

            await channel.CloseAsync();
            await connection.CloseAsync();
        }

        private static async Task ReadMessagesWithPushModel(IChannel channel)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += ConsumerReceived;

            string consumerTag = await channel.BasicConsumeAsync("my.queue1", true, consumer);

            Console.WriteLine("Subscribed. Press any key to exit. Push function");
            Console.ReadKey();

            await channel.BasicCancelAsync(consumerTag);
        }

        private static Task ConsumerReceived(object sender, BasicDeliverEventArgs basicEvent)
        {
            string message = Encoding.UTF8.GetString(basicEvent.Body.ToArray());
            Console.WriteLine($"Message:{message}");

            return Task.CompletedTask;
        }

        private static async Task ReadMessagesWithPullModel(IChannel channel)
        {
            Console.WriteLine("Reading messages. Press any key to exit.Pull method");

            while (true)
            {
                Console.WriteLine("Trying to get a message from the queue1");

                BasicGetResult? result = await channel.BasicGetAsync("my.queue1", true);

                if (result != null)
                {
                    string message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine($"Message:{message}");
                }

                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey();
                    if(keyInfo.KeyChar == 'e' || keyInfo.KeyChar == 'E')
                    {
                        return;
                    }
                }

                Thread.Sleep(2000);
            }
        }
    }
}
