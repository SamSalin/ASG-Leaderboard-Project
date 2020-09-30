using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ASG_Leaderboard_Project.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly MongoRepository _repo;

        public SimulationController(MongoRepository repo)
        {
            _repo = repo;
        }

        // ------------ SIMULATION FUNCTIONS----------------------













        [HttpGet("/simulate/season/{seasonId}/standings")]
        public async Task<List<string>> GetSeasonStandings(Guid id)
        {
            return await _repo.GetSeasonStandings(id);
        }

        [HttpGet("/simulate/season/{seasonId}/event/{eventId}/standings")]
        public async Task<List<string>> GetSeasonStandings(Guid seasonId, Guid eventId)
        {
            return await _repo.GetEventStandings(seasonId, eventId);
        }
    }
}