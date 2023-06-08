using MenuRater.Interfaces;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace MenuRater.Services.RabbitMq
{

    public class RmqPublisherService : IPublisherService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        public RmqPublisherService()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            // create connection and channel
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _circuitBreakerPolicy = Policy
            .Handle<Exception>()  // Handle all exceptions
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3, // The number of exceptions that are allowed before opening the circuit
                durationOfBreak: TimeSpan.FromMinutes(1) // The time circuit will stay open before allowing a single request
            );
        }

        //here a message could contain information about the expected return type or how to deserialize the message back.
        public Task<string> CallAsync<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            return _circuitBreakerPolicy.ExecuteAsync(() => MakeRequestAsync(message));
        }

        private async Task<string> MakeRequestAsync<T>(T message)
        {
            var replyQueueName = _channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(_channel);

            var correlationId = Guid.NewGuid().ToString();

            var props = _channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = correlationId;

            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "",
                basicProperties: props,
                body: messageBytes);

            var tcs = new TaskCompletionSource<string>();
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (eventArgs.BasicProperties.CorrelationId == correlationId)
                {
                    tcs.TrySetResult(response);
                }
            };

            _channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return await tcs.Task;
        }
    }
}
