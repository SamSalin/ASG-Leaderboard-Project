using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASG_Leaderboard_Project
{
    public class Event
    {
        [Required(ErrorMessage = "Guid is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Track is required")]
        public Track Track { get; set; }
        public List<KeyValuePair<Driver, int>> Standings { get; set; }  //Driver = driver from event, int = points from the event
    }
}