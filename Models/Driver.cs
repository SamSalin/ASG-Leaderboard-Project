using System;

namespace ASG_Leaderboard_Project
{
    public enum Team
    {
        Team1,
        Team2,
        Team3
    }

    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; } //FI,DE,GB jne
        public Team Team { get; set; } // Pitäisikö olla enum, johon on määritelty tallit?
    }
}