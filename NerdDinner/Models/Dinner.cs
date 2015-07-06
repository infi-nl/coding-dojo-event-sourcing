using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Authentication;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using NerdDinner.Events;
using Newtonsoft.Json;

namespace NerdDinner.Models
{
    public class Dinner
    {
        [HiddenInput(DisplayValue = false)]
        public int DinnerID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public Guid DinnerGuid { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title may not be longer than 50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Event Date is required")]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(256, ErrorMessage = "Description may not be longer than 256 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(20, ErrorMessage = "Hosted By name may not be longer than 20 characters")]
        [Display(Name = "Host's Name")]
        public string HostedBy { get; set; }

        [Required(ErrorMessage = "Contact info is required")]
        [StringLength(20, ErrorMessage = "Contact info may not be longer than 20 characters")]
        [Display(Name = "Contact Info")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(50, ErrorMessage = "Address may not be longer than 50 characters")]
        [Display(Name = "Address, City, State, ZIP")]
        public string Address { get; set; }

        [UIHint("CountryDropDown")]
        public string Country { get; set; }

        [HiddenInput(DisplayValue = false)]
        public double Latitude { get; set; }

        [HiddenInput(DisplayValue = false)]
        public double Longitude { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string HostedById { get; set; }

        private int _currentEvent = 0;

        private List<RSVP> _rsvps = new List<RSVP>();

        [NotMapped]
        public ICollection<RSVP> RSVPs {
            get {
                return _rsvps.AsReadOnly();
            }
        }


        public bool IsHostedBy(string userName)
        {
            return String.Equals(HostedById ?? HostedBy, userName, StringComparison.Ordinal);
        }

        public bool IsUserRegistered(string userName)
        {
            return RSVPs.Any(r => r.AttendeeNameId == userName || (r.AttendeeNameId == null && r.AttendeeName == userName));
        }

        [UIHint("LocationDetail")]
        [NotMapped]
        public LocationDetail Location
        {
            get
            {
                return new LocationDetail() { Latitude = this.Latitude, Longitude = this.Longitude, Title = this.Title, Address = this.Address };
            }
            set
            {
                this.Latitude = value.Latitude;
                this.Longitude = value.Longitude;
                this.Title = value.Title;
                this.Address = value.Address;
            }
        }

        private readonly List<string> _eventHistory = new List<string>();
        [NotMapped]
        public ICollection<string> History {
            get {
                return this._eventHistory.AsReadOnly();
            }
        }

        public ICollection<Event> RSVP(string name, string friendlyName) {
            try {
                if (IsUserRegistered(name)) {
                    return new List<Event>();
                }

                var RSVPedEvent = new RSVPed {
                    Name = name,
                    FriendlyName = friendlyName,
                    DinnerId = DinnerID
                };

                RaiseAndApply(RSVPedEvent);

                return this._publishedEvents.ToList();
            }
            finally {
                this._publishedEvents.Clear();
            }
        }


        private void RaiseAndApply(IEventData eventData) {
            var @event = MakeEvent(eventData);
            RaiseEvent(@event);
            ApplyEvent(@event);
        }

        private readonly List<Event> _publishedEvents = new List<Event>();

        private void RaiseEvent(Event e) {
            _publishedEvents.Add(e);
            _currentEvent++;
        }

        private Event MakeEvent(IEventData eventDataObject) {
            return new Event {
                AggregateId = DinnerGuid, 
                AggregateEventSequence = _currentEvent, 
                DateTime = DateTime.UtcNow, 
                EventType = eventDataObject.GetType().FullName, 
                Data = JsonConvert.SerializeObject(eventDataObject)
            };
        }

        void ApplyEvent(Event e) {
            ApplyEvent(e.AddEventType());
        }

        void ApplyEvent(Event<RSVPed> @event) {
            var rsvp = new RSVP();
            rsvp.DinnerID = this.DinnerID;
            rsvp.AttendeeName = @event.Data.FriendlyName;
            rsvp.AttendeeNameId = @event.Data.Name;
            _rsvps.Add(rsvp);
        }

        

        public void Hydrate(ICollection<Event> events)
        {
            foreach (var e in events.Where(e => e.AggregateEventSequence >= _currentEvent).OrderBy(e => e.AggregateEventSequence))
            { 
                if (e.AggregateEventSequence != _currentEvent) {
                    throw new Exception("Unexpected event sequence");
                }
                ApplyEvent(e);
                _currentEvent++;
            }
        }

        public static ICollection<Dinner> HydrateAll(List<Dinner> dinners, List<Event> events) {
            foreach (var dinner in dinners) {
                dinner.Hydrate(events.Where(e => e.AggregateId == dinner.DinnerGuid).ToList());
            }
            return dinners;
        }


    }

    public class LocationDetail
    {
        public double Latitude;
        public double Longitude;
        public string Title;
        public string Address;
    }
}