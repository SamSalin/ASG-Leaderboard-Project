using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASG_Leaderboard_Project
{
    public class Season
    {
        [Required(ErrorMessage = "Guid is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public int CurrentEventIndex { get; set; } //Check which event is up next in the season
        public List<Event> Events { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<KeyValuePair<Driver, int>> Standings { get; set; } //Driver = driver from season, int = accumulated points throughout season
    }
}