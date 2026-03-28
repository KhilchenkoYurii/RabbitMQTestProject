using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FanoutPublisher
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

            await channel.QueueDeclareAsync(
                "my.queue1",
                true,
                false,
                false,
                null);

            await channel.QueueDeclareAsync(
                "my.queue2",
                true,
                false,
                false,
                null);

            await channel.QueueBindAsync("my.queue1", "ex.fanout", "");
            await channel.QueueBindAsync("my.queue2", "ex.fanout", "");

            await channel.BasicPublishAsync(
                "ex.fanout",
                "",
                Encoding.UTF8.GetBytes("Message1"));

            await channel.BasicPublishAsync(
                "ex.fanout",
                "",
                Encoding.UTF8.GetBytes("Message2"));

            Console.WriteLine("Waiting for messages. Press any key to exit.Publisher");
            Console.ReadKey();

            await channel.QueueDeleteAsync("my.queue1");
            await channel.QueueDeleteAsync("my.queue2");

            await channel.ExchangeDeleteAsync("ex.fanout");

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
