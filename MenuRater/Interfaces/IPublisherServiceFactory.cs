namespace MenuRater.Interfaces
{
    public interface IPublisherServiceFactory
    {
        IPublisherService GetPublisher<T>(T message);
    }
}
