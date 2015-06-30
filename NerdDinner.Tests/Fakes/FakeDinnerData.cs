using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdDinner.Models;
using System.Web.Mvc;

namespace NerdDinner.Tests.Fakes
{
    class FakeDinnerData
    {
        public static Tuple<List<Dinner>, List<Event>> CreateTestDinners()
        {

            List<Dinner> dinners = new List<Dinner>();
            List<Event> events = new List<Event>();

            for (int i = 1; i <= 101; i++)
            {


                Dinner sampleDinner = new Dinner()
                {
                    DinnerID = i,
                    DinnerGuid = Guid.NewGuid(),
                    Title = "Sample Dinner",
                    HostedBy = "SomeUser",
                    Address = "Some Address",
                    Country = "USA",
                    ContactPhone = "425-555-1212",
                    Description = "Some description",
                    EventDate = DateTime.Now.AddDays(i),
                    Latitude = 99,
                    Longitude = -99
                };
                dinners.Add(sampleDinner);

                var e = new Event();
                e.Data = @"{""Name"":""SomeUser"",""FriendlyName"":""SomeUser""}";
                e.EventType = "NerdDinner.Events.RSVPed";
                e.DateTime = DateTime.UtcNow;
                e.AggregateId = sampleDinner.DinnerGuid;
                events.Add(e);
            }

            return Tuple.Create(dinners, events);
        }

        public static Dinner CreateDinner()
        {
            Dinner dinner = new Dinner();
            dinner.Title = "New Test Dinner";
            dinner.EventDate = DateTime.Now.AddDays(7);
            dinner.Address = "5 Main Street";
            dinner.Description = "Desc";
            dinner.ContactPhone = "503-555-1212";
            dinner.HostedBy = "scottgu";
            dinner.Latitude = 45;
            dinner.Longitude = 45;
            dinner.Country = "USA";
            return dinner;
        }

        public static FormCollection CreateDinnerFormCollection()
        {
            var form = new FormCollection();

            form.Add("Description", "Description");
            form.Add("Title", "New Test Dinner");
            form.Add("EventDate", "2010-02-14");
            form.Add("Address", "5 Main Street");
            form.Add("ContactPhone", "503-555-1212");
            return form;
        }

    }
}
