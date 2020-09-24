using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class ModifiedSeason
    {
        public string name { get; set; }
        public int currentEventIndex { get; set; }
        public List<Event> Events { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<KeyValuePair<Driver, int>> Standings { get; set; }
    }
}