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

        [HttpGet("/simulate/season/{seasonId}/lastevent")]
        public async Task<string> LastEvent(Guid seasonId)
        {
            return await _repo.LastEvent(seasonId);
        }

        [HttpGet("/simulate/season/{seasonId}/nextevent")]
        public async Task<string> NextEvent(Guid seasonId)
        {
            return await _repo.NextEvent(seasonId);
        }

        [HttpGet("/simulate/season/{id}")]
        public async Task<String> SimulateNextEvent(Guid id)
        {
            return await _repo.SimulateNextEvent(id);
        }
        [HttpGet("/simulate/season/{id}/standings")]
        public async Task<List<string>> GetSeasonStandings(Guid id)
        {
            return await _repo.GetSeasonStandings(id);
        }

        [HttpGet("/simulate/season/{seasonId}/event/{eventId}/standings")]
        public async Task<List<string>> GetEventStandings(Guid seasonId, Guid eventId)
        {
            return await _repo.GetEventStandings(seasonId, eventId);
        }
    }
}