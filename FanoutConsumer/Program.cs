using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanoutConsumer
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

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += ConsumerReceived;

            await channel.BasicConsumeAsync("my.queue2", true, consumer);

            Console.WriteLine("Waiting for messages. Press any key to exit.Consumer");
            Console.ReadKey();
        }

        private static Task ConsumerReceived(object sender, BasicDeliverEventArgs basicEvent)
        {
            string message = Encoding.UTF8.GetString(basicEvent.Body.ToArray());
            Console.WriteLine($"Message:{message}");

            return Task.CompletedTask;
        }
    }
}
