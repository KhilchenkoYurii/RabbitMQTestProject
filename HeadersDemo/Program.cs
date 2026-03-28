using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadersDemo
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
                "ex.headers",
                "headers",
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

            await channel.QueueBindAsync(
                "my.queue1",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match","all" },
                    {"job","convert" },
                    {"type", "jpeg" }
                });

            await channel.QueueBindAsync(
                "my.queue2",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match","any" },
                    {"job","convert" },
                    {"type", "jpeg" }
                });

            var properties1 = new BasicProperties()
            {
                Headers = new Dictionary<string, object>
                {
                    {"job","convert" },
                    {"type", "jpeg" }
                }
            };

            var properties2 = new BasicProperties()
            {
                Headers = new Dictionary<string, object>
                {
                    {"job","convert" },
                    {"type", "jpg" }
                }
            };

            await channel.BasicPublishAsync(
                exchange: "ex.headers",
                routingKey: "",
                false,
                basicProperties: properties1,
                body: Encoding.UTF8.GetBytes("Message with all headers")
            );

            await channel.BasicPublishAsync(
                exchange: "ex.headers",
                routingKey: "",
                false,
                basicProperties: properties2,
                body: Encoding.UTF8.GetBytes("Message with one match headers")
            );
        }
    }
}
