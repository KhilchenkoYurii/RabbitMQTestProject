using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExToExDemo
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
                "exchange1",
                "direct",
                true,
                false,
                null);

            await channel.ExchangeDeclareAsync(
                "exchange2",
                "direct",
                true,
                false,
                null);

            await channel.QueueDeclareAsync(
                "queue1",
                true,
                false,
                false,
                null);
            await channel.QueueDeclareAsync(
                "queue2",
                true,
                false,
                false,
                null);

            await channel.QueueBindAsync("queue1", "exchange1", "key1");
            await channel.QueueBindAsync("queue2", "exchange2", "key2");
            await channel.ExchangeBindAsync("exchange2", "exchange1", "key2");

            await channel.BasicPublishAsync(
                "exchange1",
                "key1",
                Encoding.UTF8.GetBytes("Message with key 1"));

            await channel.BasicPublishAsync(
                "exchange1",
                "key2",
                Encoding.UTF8.GetBytes("Message with key2"));
        }
    }
}
