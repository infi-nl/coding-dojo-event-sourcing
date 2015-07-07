using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using NerdDinner.Events;
using Newtonsoft.Json;

namespace NerdDinner.Models
{
    public static class RSVPEventHandler {

        public static void OnEventPublished(NerdDinners dbContext, Event @event) {
            if (@event.EventType.Equals("NerdDinner.Events.RSVPed")) {
                HandleRSVPed(dbContext, @event);
            }
            else if (@event.EventType.Contains("NerdDinner.EventsRSVPCanceled")) {
                HandleRSVPCanceled(dbContext, @event);
            }
            
        }

        private static void HandleRSVPed(NerdDinners dbContext, Event e) {
            var query = String.Format(
                     @"MERGE INTO [RSVPCounts] WITH (HOLDLOCK) AS R
					        USING (SELECT [DinnerGuid] FROM [Dinners] WHERE [DinnerGuid] = '{0}') AS R_new
					        ON R.[DinnerGuid] = R_new.[DinnerGuid]
					   WHEN MATCHED THEN UPDATE SET [Count] = [Count] + 1
					   WHEN NOT MATCHED THEN INSERT ([DinnerGuid], [Count]) VALUES (R_new.[DinnerGuid], 1);", e.AggregateId);

            var resultCode = dbContext.Database.ExecuteSqlCommand(query);
        }

        private static void HandleRSVPCanceled(NerdDinners dbContext, Event e) {
            var query = String.Format(
                     @"MERGE INTO [RSVPCounts] WITH (HOLDLOCK) AS R
					        USING (SELECT [DinnerGuid] FROM [Dinners] WHERE [DinnerGuid] = '{0}') AS R_new
					        ON R.[DinnerGuid] = R_new.[DinnerGuid]
					   WHEN MATCHED THEN UPDATE SET [Count] = [Count] - 1", e.AggregateId);

            var resultCode = dbContext.Database.ExecuteSqlCommand(query);
        }
    }
}
