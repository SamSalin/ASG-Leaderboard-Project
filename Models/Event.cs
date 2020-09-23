using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class Event
    {
        public Guid id;
        public DateTime date;
        public string name;
        public Track track;
        public List<KeyValuePair<Guid, int>> standings;
    }
}