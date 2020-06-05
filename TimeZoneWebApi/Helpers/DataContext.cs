using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TimeZoneWebApi.Entities;

namespace TimeZoneWebApi.Helpers
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("TimeZoneWebApiDatabase"));
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Entities.TimeZone> TimeZones { get; set; }
    }
}