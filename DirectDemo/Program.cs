using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectDemo
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
                "ex.direct",
                "direct",
                true,
                false,
                null);

            await channel.QueueDeclareAsync(
                "my.warnings",
                true,
                false,
                false,
                null);
            await channel.QueueDeclareAsync(
                "my.errors",
                true,
                false,
                false,
                null);
            await channel.QueueDeclareAsync(
                "my.infos",
                true,
                false,
                false,
                null);

            await channel.QueueBindAsync("my.infos", "ex.direct", "info");
            await channel.QueueBindAsync("my.warnings", "ex.direct", "warning");
            await channel.QueueBindAsync("my.errors", "ex.direct", "error");

            await channel.BasicPublishAsync(
                "ex.direct",
                "info",
                Encoding.UTF8.GetBytes("Message with info header"));

            await channel.BasicPublishAsync(
                "ex.direct",
                "warning",
                Encoding.UTF8.GetBytes("Message with warning header"));

            await channel.BasicPublishAsync(
                "ex.direct",
                "error",
                Encoding.UTF8.GetBytes("Message with error header"));

        }
    }
}
