using System;

namespace ASG_Leaderboard_Project
{
    public class ModifiedDriver
    {
        public string Name { get; set; }
        public string Nationality { get; set; } //FI,DE,GB jne
        public Team Team { get; set; } // Pitäisikö olla enum, johon on määritelty tallit?
    }
}