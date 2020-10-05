using System;
using System.ComponentModel.DataAnnotations;

namespace ASG_Leaderboard_Project
{
    public class Track
    {
        [Required(ErrorMessage = "Guid is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(2, ErrorMessage = "The track country value cannot exceed 2 characters.")]
        public string Country { get; set; }

    }
}