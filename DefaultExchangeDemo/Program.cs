using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultExchangeDemo
{
    internal class Program
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

            await channel.BasicPublishAsync(
                "",
                "my.queue2",
                Encoding.UTF8.GetBytes("Message for queue 2"));

            await channel.BasicPublishAsync(
                "",
                "my.queue1",
                Encoding.UTF8.GetBytes("Message for queue 1"));
        }
    }
}
