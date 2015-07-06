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

    public class CreateDatabaseIfNotExistsIncludingUniqueIndices : CreateDatabaseIfNotExists<NerdDinners>
    {
        protected override void Seed(NerdDinners context) {
            base.Seed(context);
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX IX_UQ_AggergateId_AggregateEventSequence ON dbo.Events ( AggregateId, AggregateEventSequence ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX IX_UQ_DinnerGuid ON dbo.Dinners ( DinnerGuid ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }
    }

}