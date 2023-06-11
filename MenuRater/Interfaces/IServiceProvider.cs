namespace MenuRater.Interfaces
{
    public interface IServiceProvider
    {
        Task<string> CallAsync<T>(T message);
    }
}
