using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class Season
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int CurrentEventIndex { get; set; } //Check which event is up next in the season
        public List<Event> Events { get; set; }

        public List<Driver> Drivers { get; set; }

        public List<KeyValuePair<Guid, int>> Standings { get; set; } //Is it the best possible solution?

    }
}