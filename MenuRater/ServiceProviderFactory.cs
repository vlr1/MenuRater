using MenuRater.Interfaces;
using MenuRater.Services.Http;
using MenuRater.Services.RabbitMq;
using Messaging.Models;

namespace MenuRater
{
    public class ServiceProviderFactory : IServiceProviderFactory
    {
        private readonly System.IServiceProvider _serviceProvider;

        public ServiceProviderFactory(System.IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Interfaces.IServiceProvider GetServiceProvider<T>(T message)
        {
            if (typeof(T) == typeof(Message))
            {
                return _serviceProvider.GetService<RmqServiceProvider>();
            }
            else if (typeof(T) == typeof(HttpRequestMessage))
            {
                return _serviceProvider.GetService<HttpServiceProvider>();
            }

            throw new ArgumentException($"Unsupported message type: {typeof(T)}");
        }
    }
}
