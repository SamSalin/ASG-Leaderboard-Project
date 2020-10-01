using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project
{
    public class ModifiedEvent
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public ModifiedTrack Track { get; set; }
    }
}