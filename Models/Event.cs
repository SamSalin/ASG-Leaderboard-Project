using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class Event
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public Track Track { get; set; }

        public List<KeyValuePair<Driver, int>> Standings { get; set; }  //Driver = driver from event, int = points from the event

    }
}