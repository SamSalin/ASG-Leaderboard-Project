using System;
using System.Collections.Generic;

namespace ASG_Leaderboard_Project.Models
{
    public class Season
    {
        Guid id;
        string name;
        int nextLevelIndex;                             //Check which event is up next in the season
        List<Event> events;
        List<Driver> drivers;
        List<KeyValuePair<Guid, int>> standings;      //Is it the best possible solution?

    }
}