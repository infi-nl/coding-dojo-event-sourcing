using System;

namespace NerdDinner.Models
{
    public class Event {

        public int            Id          { get; set; }
        public Guid           AggregateId { get; set; }
        public string         Data        { get; set; }

        public string EventType { get; set; }

        public DateTimeOffset DateTime    { get; set; }
    }  
}