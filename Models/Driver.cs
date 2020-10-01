using System;

namespace ASG_Leaderboard_Project
{
    public enum Team
    {
        team1,
        Team2,
        Team3
    }

    public class Driver
    {
        [Required(ErrorMessage = "Guid is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [DataType(DataType.Text)]
        public string Nationality { get; set; } //FI,DE,GB jne
        public Team Team { get; set; } // Pitäisikö olla enum, johon on määritelty tallit?
    }
}