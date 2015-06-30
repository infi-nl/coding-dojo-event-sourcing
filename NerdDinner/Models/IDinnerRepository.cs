using System.Collections.Generic;
using System.Linq;

namespace NerdDinner.Models {

    public interface IDinnerRepository : IRepository<Dinner>
    {
        IQueryable<Dinner> FindByLocation(float latitude, float longitude);
        IQueryable<Dinner> FindUpcomingDinners();
        IQueryable<Dinner> FindDinnersByText(string q);
        

        void StoreEventsForDinner(Dinner dinner);
    }
}
