using RabbitMQ.Client;
using System.Text;

namespace PublisherExample
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

            while (true)
            {
                Console.WriteLine("Enter message");
                var message = Console.ReadLine();

                if(message == "exit")
                {
                    break;
                }

                await channel.BasicPublishAsync(
                    "ex.fanout",
                    "",
                    Encoding.UTF8.GetBytes(message ?? "message example"));

            }

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
