﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AmsApi.Models
{
    public class EventLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }

        public string Event { get; set; }


        public string Category { get; set; }

        public int EventTriggeredBy { get; set; }

        public DateTime LoggedAT { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        public string OldValue { get; set; }  // New column for old value
        public string NewValue { get; set; }  // New column for new value
    }
}
