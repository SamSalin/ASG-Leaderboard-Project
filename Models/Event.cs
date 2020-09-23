using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project.Models
{
    public class Event
    {
        Guid id;
        DateTime date;
        string name;
        Track track;
        List<KeyValuePair<Guid, int>> standings;
    }
}