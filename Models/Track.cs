using System;

namespace ASG_Leaderboard_Project
{
    public class Track
    {
        [Required(ErrorMessage = "Guid is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Guid is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public string Country { get; set; }

    }
}