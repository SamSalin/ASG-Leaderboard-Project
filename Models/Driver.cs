using System;

namespace ASG_Leaderboard_Project
{
    public class Driver
    {
        public Guid Dd { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; } //FI,DE,GB jne
        public string Team { get; set; } // Pitäisikö olla enum, johon on määritelty tallit?
    }
}