using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace NerdDinner.Models
{
    public interface INerdDinners
    {
        Database Database { get; }
        int SaveChanges();
        DbEntityEntry Entry(object entity);

        DbSet<Dinner> Dinners { get; set; }
        DbSet<Models.Event> Events { get; set; }
    }

    public class NerdDinners : DbContext, INerdDinners
    {
        public NerdDinners() 
        {
            Configuration.LazyLoadingEnabled = false;  
        }

        public DbSet<Dinner> Dinners { get; set; }
        
        public DbSet<Event> Events { get; set; }

    }
}