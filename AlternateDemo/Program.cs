using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternateDemo
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

            await channel.ExchangeDeclareAsync(
                "ex.fanout",
                "fanout",
                true,
                false,
                null);

            await channel.ExchangeDeclareAsync(
                "ex.direct",
                "direct",
                true,
                false,
                new Dictionary<string, object?>()
                {
                    {"alternate-exchange","ex.fanout" }
                });

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
            await channel.QueueDeclareAsync(
                "my.unrouted",
                true,
                false,
                false,
                null);

            await channel.QueueBindAsync("my.queue1", "ex.direct", "key1");
            await channel.QueueBindAsync("my.queue2", "ex.direct", "key2");
            await channel.QueueBindAsync("my.unrouted", "ex.fanout", "");

            await channel.BasicPublishAsync(
                "ex.direct",
                "key1",
                Encoding.UTF8.GetBytes("Message with key1"));

            await channel.BasicPublishAsync(
                "ex.direct",
                "key2",
                Encoding.UTF8.GetBytes("Message with key2"));

            await channel.BasicPublishAsync(
                "ex.direct",
                "key3",
                Encoding.UTF8.GetBytes("Message with key3"));

        }
    }
}
