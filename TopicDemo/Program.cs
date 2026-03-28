using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicDemo
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
                "ex.topic",
                "topic",
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
            await channel.QueueDeclareAsync(
                "my.queue3",
                true,
                false,
                false,
                null);

            await channel.QueueBindAsync("my.queue1", "ex.topic", "*.image.*");
            await channel.QueueBindAsync("my.queue2", "ex.topic", "#.image");
            await channel.QueueBindAsync("my.queue3", "ex.topic", "image.#");

            await channel.BasicPublishAsync(
                "ex.topic",
                "something.image.something",
                Encoding.UTF8.GetBytes("Message with something.image.something header"));

            await channel.BasicPublishAsync(
                "ex.topic",
                "something.something.image",
                Encoding.UTF8.GetBytes("Message with something.something.image header"));

            await channel.BasicPublishAsync(
                "ex.topic",
                "image.something.something",
                Encoding.UTF8.GetBytes("Message with image.something.something header"));

            await channel.BasicPublishAsync(
                "ex.topic",
                "image",
                Encoding.UTF8.GetBytes("Message with image header"));

        }
    }
}
