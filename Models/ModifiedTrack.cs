using System;

namespace ASG_Leaderboard_Project
{
    public class ModifiedTrack
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [DataType(DataType.Text)]
        public string Country { get; set; }
    }
}