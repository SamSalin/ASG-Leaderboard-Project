using System;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Driver guid is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Driver name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(2, ErrorMessage = "The driver nationality value cannot exceed 2 characters.")]
        public string Nationality { get; set; } //FI,DE,GB
        public Team Team { get; set; }
    }
}