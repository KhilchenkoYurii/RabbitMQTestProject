using Demo.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using Constants = Demo.Common.Constants;

namespace Requestor
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            ConcurrentDictionary<string, CalculationRequest> waitingRequests = new ConcurrentDictionary<string,CalculationRequest>();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            var connection = await factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            string responseQueueName = $"res.{Guid.NewGuid().ToString()}";

            await channel.QueueDeclareAsync(responseQueueName);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, e) => {

                string requestId = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers[Constants.RequestHeaderKey]);

                CalculationRequest request;

                if (waitingRequests.TryGetValue(requestId, out request))
                {
                    string messageData = Encoding.UTF8.GetString(e.Body.ToArray());
                    CalculationResponse response = JsonConvert.DeserializeObject<CalculationResponse>(messageData);

                    Console.WriteLine($"Calculation result: {request.ToString()}={response.ToString()}");

                }
            };

            await channel.BasicConsumeAsync(responseQueueName, true, consumer);

            Console.WriteLine("Press any key to send request");
            Console.ReadKey();

            SendMessage(waitingRequests, channel, new CalculationRequest(2, 4, OperationType.Add), responseQueueName);
            SendMessage(waitingRequests, channel, new CalculationRequest(4, 2, OperationType.Substract), responseQueueName);
            SendMessage(waitingRequests, channel, new CalculationRequest(1, 1, OperationType.Add), responseQueueName);
            SendMessage(waitingRequests, channel, new CalculationRequest(6, 4, OperationType.Substract), responseQueueName);

            Console.WriteLine("Waiting for messages. Press any key to exit.Consumer");
            Console.ReadKey();

            await channel.CloseAsync();
            await connection.CloseAsync();
        }

        private static async void SendMessage(ConcurrentDictionary<string,
            CalculationRequest> waitingRequests,
            IChannel? channel, 
            CalculationRequest request,
            string responseQueueName)
        {
            string requestId = Guid.NewGuid().ToString();
            string requestData = JsonConvert.SerializeObject(request);

            waitingRequests[requestId] = request;

            var basicProperty = new BasicProperties()
            {
                Headers = new Dictionary<string, object?>
                {
                    {Constants.RequestHeaderKey,Encoding.UTF8.GetBytes(requestId) },
                    {Constants.ResponseQueueHeaderKey,Encoding.UTF8.GetBytes(responseQueueName) },
                }
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "requests",
                false,
                basicProperties: basicProperty,
                body: Encoding.UTF8.GetBytes(requestData)
            );

        }
    }
}