using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class Season
    {
        public Guid id;
        public string name;
        public int currentEventIndex;                             //Check which event is up next in the season
        public List<Event> events;
        public List<Driver> drivers;
        public List<KeyValuePair<Guid, int>> standings;      //Is it the best possible solution?

    }
}