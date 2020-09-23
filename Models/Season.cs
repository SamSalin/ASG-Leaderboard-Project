using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class Season
    {
        public Guid Id;
        public string Name;
        public int CurrentEventIndex;                             //Check which event is up next in the season
        public List<Event> Events;
        public List<Driver> Drivers;
        public List<KeyValuePair<Guid, int>> Standings;      //Is it the best possible solution?

    }
}