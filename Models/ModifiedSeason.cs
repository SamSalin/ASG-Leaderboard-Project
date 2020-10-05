using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASG_Leaderboard_Project
{
    public class ModifiedSeason
    {
        [Required(ErrorMessage = "Season name is required")]
        [DataType(DataType.Text)]
        public string name { get; set; }
        public int currentEventIndex { get; set; }
        public List<Event> Events { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<KeyValuePair<Driver, int>> Standings { get; set; }
    }
}