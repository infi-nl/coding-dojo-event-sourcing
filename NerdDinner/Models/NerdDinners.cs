using System;
using System.Collections.Generic;
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

        readonly static List<Action<NerdDinners,Event>> _eventPublishedHandlers = new List<Action<NerdDinners,Event>>();

        public static void OnEventsPublished(Action<NerdDinners,Event> handler) {
            lock(_eventPublishedHandlers) {
                _eventPublishedHandlers.Add(handler);
            }
        }

        public static void ClearEventHandlers() {
            _eventPublishedHandlers.Clear();
        }

        internal void StoreEvents(ICollection<Event> events) {
            foreach (var publishedEvent in events) {
                Events.Add(publishedEvent);
            }
            foreach(var handler in _eventPublishedHandlers) {
                foreach(var @event in events) {
                    handler(this,@event);
                }
            }
        }
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