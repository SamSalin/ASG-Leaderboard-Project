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

        [HttpPost("/seasons/{id}/event/create")]
        public async Task<Event> CreateEvent(Guid id, [FromBody] NewEvent newEvent)
        {
            Event createdEvent = new Event()
            {
                Id = Guid.NewGuid(),
                Name = newEvent.Name,
                Date = newEvent.Date,
                Track = CreateTrack(newEvent.Track),
                Standings = new List<KeyValuePair<Driver, int>>()
            };

            return await _repo.CreateEvent(id, createdEvent);
        }
        public Track CreateTrack(NewTrack newTrack)
        {
            Track track = new Track()
            {
                Id = Guid.NewGuid(),
                Name = newTrack.Name,
                Country = newTrack.Country
            };

            return track;
        }

        [HttpGet("/seasons/get/{id}")]
        public async Task<Season> GetSeason(Guid id)
        {
            return await _repo.GetSeason(id);
        }

        [HttpGet("/seasons/get/{seasonId}/event/{eventId}")]
        public async Task<Event> GetSeasonEvent(Guid seasonId, Guid eventId)
        {
            return await _repo.GetSeasonEvent(seasonId, eventId);
        }

        [HttpGet("/seasons/getall")]
        public async Task<Season[]> GetAllSeasons()
        {
            return await _repo.GetAllSeasons();
        }

        [HttpDelete("/seasons/delete/{id}")]
        public async Task<Season> DeleteSeason(Guid id)
        {
            return await _repo.DeleteSeason(id);
        }

        [HttpPut("/seasons/modify/{id}/index")]
        public async Task<Season> ModifySeasonIndex(Guid id, [FromQuery] int index)
        {
            return await _repo.ModifySeasonIndex(id, index);
        }

        [HttpPut("/seasons/modify/{seasonId}/event/{eventId}")]
        public async Task<Season> ModifySeasonEvent(Guid seasonid, Guid eventId, [FromBody] ModifiedEvent modifiedEvent)
        {

            return await _repo.ModifySeasonEvent(seasonid, eventId, modifiedEvent);
        }
    }
}