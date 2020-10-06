using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ASG_Leaderboard_Project.Controllers
{
    /*--------------------------------------------------------------
        Simulation controller includes all HTTP request methods needed to
        simulate a full season. These methods are intended to be called
        by the end users. Methods in this document return a simple
        string in a easily readable format. 
    */

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

        [HttpGet("/simulate/season/{id}/all")]
        public async Task<String> SimulateRestofTheSeason(Guid id)
        {
            return await _repo.SimulateRestofTheSeason(id);
        }

        [HttpGet("/simulate/season/{id}/standings")]
        public async Task<string> GetSeasonStandings(Guid id)
        {
            return await _repo.GetSeasonStandings(id);
        }

        [HttpGet("/simulate/season/{seasonId}/event/{eventId}/standings")]
        public async Task<string> GetEventStandings(Guid seasonId, Guid eventId)
        {
            return await _repo.GetEventStandings(seasonId, eventId);
        }

        [HttpGet("/simulate/season/{seasonId}/driver/{driverId}/standings")]
        public async Task<string> GetDriverStandings(Guid seasonId, Guid driverId)
        {
            return await _repo.GetDriverStandings(seasonId, driverId);
        }

        [HttpGet("/simulate/season/{seasonId}/compare/{driverId1}/{driverId2}")]
        public async Task<string> CompareDrivers(Guid seasonId, Guid driverId1, Guid driverId2)
        {
            return await _repo.CompareDrivers(seasonId, driverId1, driverId2);
        }
    }
}