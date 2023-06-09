using MenuRater.Data;

namespace MenuRater.Interfaces
{
    public interface IDataContextFactory
    {
        DataContext Create();
    }
}
