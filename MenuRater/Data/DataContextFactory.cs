using MenuRater.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MenuRater.Data
{
    public class DataContextFactory : IDataContextFactory
    {
        private readonly DbContextOptions<DataContext> _options;

        public DataContextFactory(DbContextOptions<DataContext> options)
        {
            _options = options;
        }

        public DataContext Create()
        {
            return new DataContext(_options);
        }
    }
}
