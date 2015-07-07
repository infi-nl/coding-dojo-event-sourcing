using System.Collections;
using NerdDinner.Events;
using NerdDinner.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdDinner.Controllers;

namespace NerdDinner.Tests.CodingDojo {
    partial class DojoTests {
        [Test]
        public void GetPopularDinners_Is_Populated_From_PopularDinners_ReadModel() {
            PopulatePopularDinnerReadModelForDinner(1, 10);
            PopulatePopularDinnerReadModelForDinner(3, 5);
            PopulatePopularDinnerReadModelForDinner(4, 15);

            var result = CreateSearchControllerAs("freek").GetMostPopularDinners(10);

            var model = GetDataFromJsonResult<ICollection<JsonDinner>>(result);

            var firstResult = model.First();

            Assert.AreEqual(4,firstResult.DinnerID,"most popular dinner is not correct");
            Assert.AreEqual(15, firstResult.RSVPCount, "RSVP count is wrong");
        }

        private void PopulatePopularDinnerReadModelForDinner(int dinnerId, int rsvpCount)
        {
            var ctx = new NerdDinners();
            var pop = PopularDinnerFromDinner(ctx.Dinners.Find(dinnerId));

            pop.RSVPCount = rsvpCount;
            ctx.PopularDinners.Add(pop);

            ctx.SaveChanges();
        }

        private PopularDinner PopularDinnerFromDinner(Dinner dinner)
        {
            var result = new PopularDinner();
            foreach(var prop in typeof(Dinner).GetProperties()) {
                var propOnPopularDinner = typeof(PopularDinner).GetProperties().FirstOrDefault(p => p.Name == prop.Name);
                if(propOnPopularDinner==null) {
                    continue; 
                }
                propOnPopularDinner.SetValue(result,prop.GetValue(dinner,null),null);
            }

            return result;
        }
    }
}
