using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ASG_Leaderboard_Project
{

    [ApiController]
    [Route("[controller]")]
    public class SeasonController : ControllerBase
    {
        private readonly MongoRepository _repo;

        public SeasonController(MongoRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("/seasons/create")]
        public async Task<Season> CreateSeason([FromQuery] string seasonName)
        {
            Season season = new Season()
            {
                id = Guid.NewGuid(),
                name = seasonName,
                currentEventIndex = 0,
                events = new List<Event>(),
                drivers = new List<Driver>(),
                standings = new List<KeyValuePair<Guid, int>>()
            };

            return await _repo.CreateSeason(season);
        }
    }
}