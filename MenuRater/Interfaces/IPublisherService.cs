namespace MenuRater.Interfaces
{
    public interface IPublisherService
    {
        Task<string> CallAsync<T>(T message);
    }
}
