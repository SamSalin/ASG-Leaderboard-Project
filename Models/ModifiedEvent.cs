using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASG_Leaderboard_Project
{
    public class ModifiedEvent
    {
        [Required(ErrorMessage = "Event name is required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Event track is required")]
        public ModifiedTrack Track { get; set; }
    }
}