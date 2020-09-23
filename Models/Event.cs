using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class Event
    {
        public Guid Id;
        public DateTime Date;
        public string Name;
        public Track Track;
        public List<KeyValuePair<Guid, int>> Standings;
    }
}