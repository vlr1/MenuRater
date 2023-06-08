using MenuRater.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuRater.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<MenuRate> MenuRates { get; set; }
    }
}
