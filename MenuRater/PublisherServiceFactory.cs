using Azure.Core;
using MenuRater.Interfaces;
using MenuRater.Services.Http;
using MenuRater.Services.RabbitMq;
using Messaging.Models;

namespace MenuRater
{
    public class PublisherServiceFactory : IPublisherServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PublisherServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPublisherService GetPublisher<T>(T message)
        {
            if (typeof(T) == typeof(Message))
            {
                return _serviceProvider.GetService<RmqPublisherService>();
            }
            else if (typeof(T) == typeof(HttpRequestMessage))
            {
                return _serviceProvider.GetService<HttpPublisherService>();
            }

            throw new ArgumentException($"Unsupported message type: {typeof(T)}");
        }
    }
}
