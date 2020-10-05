using System;
using System.ComponentModel.DataAnnotations;

namespace ASG_Leaderboard_Project
{
    public class ModifiedDriver
    {
        [Required(ErrorMessage = "Driver name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(2, ErrorMessage = "The driver nationality value cannot exceed 2 characters.")]
        public string Nationality { get; set; } //FI,DE,GB

        [EnumDataType(typeof(Team), ErrorMessage = "Invalid Team")]
        public Team Team { get; set; }
    }
}