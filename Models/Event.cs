using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class Event
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; }

        public Track Track { get; set; }

        public List<KeyValuePair<Guid, int>> Standings { get; set; }

    }
}