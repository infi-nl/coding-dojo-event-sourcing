using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Services.Common;

namespace NerdDinner.Models
{
    public class RSVPCount
    {
        [Key]
        public Guid DinnerGuid { get; set; }

        public int Count { get; set; }
    }
}