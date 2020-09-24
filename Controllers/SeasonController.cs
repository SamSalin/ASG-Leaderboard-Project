using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ASG_Leaderboard_Project.Controllers
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
                Id = Guid.NewGuid(),
                Name = seasonName,
                CurrentEventIndex = 0,
                Events = new List<Event>(),
                Drivers = new List<Driver>(),
                Standings = new List<KeyValuePair<Driver, int>>()
            };

            return await _repo.CreateSeason(season);
        }

        [HttpGet("/seasons/get/{id}")]
        public async Task<Season> GetSeason(Guid id)
        {
            return await _repo.GetSeason(id);
        }

        [HttpGet("/seasons/getall")]
        public async Task<Season[]> GetAllSeasons()
        {
            return await _repo.GetAllSeasons();
        }
    }
}