using Microsoft.EntityFrameworkCore;
using yapp.Domain;

namespace yapp.Infrastructure
{
    public class YappDbContext : DbContext
    {
        private readonly string _databaseName;

        public YappDbContext(string databaseName)
        {
            _databaseName = databaseName;
        }

        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databaseName}");
        }
    }
}
