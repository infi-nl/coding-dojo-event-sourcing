using System.ComponentModel.DataAnnotations;
using System.Data.Services.Common;

namespace NerdDinner.Models
{
    public class RSVP
    {
        public int DinnerID { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeNameId { get; set; }
    }
}