namespace MenuRater.Interfaces
{
    public interface IServiceProviderFactory
    {
        IServiceProvider GetServiceProvider<T>(T message);
    }
}
