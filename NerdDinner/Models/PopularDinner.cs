using NerdDinner.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace NerdDinner.Models
{
    public class PopularDinner
    {
        //TODO: add concurrency control

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DinnerID { get; set; }
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public string HostedBy { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string HostedById { get; set; }

        public int RSVPCount { get; set; }

        public static void Handle(NerdDinners context, Event @event) {
            if(Type.GetType(@event.EventType) ==  typeof(RSVPed)) {
                Handle(context,(Event<RSVPed>)@event.AddEventType());
            }
        }

        private static void Handle(NerdDinners context,Event<RSVPed> @event) {
            var popular = context.PopularDinners.Find(@event.Data.DinnerId);
            if(popular == null) {
                throw new InvalidOperationException("Couldn't find dinner with id "+@event.Data.DinnerId);
            }

            popular.RSVPCount++;
        }
    }
}
