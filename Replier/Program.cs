using Demo.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Constants = Demo.Common.Constants;

namespace Replier
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

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, e) => 
            {
                string requestData = Encoding.UTF8.GetString(e.Body.ToArray());
                CalculationRequest request = JsonConvert.DeserializeObject<CalculationRequest>(requestData);

                Console.WriteLine($"Request received: {request.ToString()}");

                CalculationResponse response = new CalculationResponse();

                response.Result = request.Operation == OperationType.Add 
                                        ? request.Number1 + request.Number2 
                                        : request.Number1 - request.Number2;

                string responseData = JsonConvert.SerializeObject(response);

                var basicProperty = new BasicProperties()
                {
                    Headers = new Dictionary<string, object?>
                {
                    {Constants.RequestHeaderKey, e.BasicProperties.Headers[Constants.RequestHeaderKey] },
                }
                };

                string responseQueueName = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers[Constants.RequestHeaderKey]);

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: responseQueueName,
                    false,
                    basicProperties: basicProperty,
                    body: Encoding.UTF8.GetBytes(responseData)
                );
            };

            await channel.BasicConsumeAsync("requests", true, consumer);

            Console.WriteLine("Waiting for messages. Press any key to exit.Consumer");
            Console.ReadKey();

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
