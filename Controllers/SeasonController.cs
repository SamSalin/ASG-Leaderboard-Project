using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ASG_Leaderboard_Project.Controllers
{
    /*--------------------------------------------------------------
        Season controller includes all necessary CRUD-operations for
        maintaining a season database
    */

    [ApiController]
    [Route("[controller]")]

    public class SeasonController : ControllerBase
    {
        private readonly MongoRepository _repo;

        public SeasonController(MongoRepository repo)
        {
            _repo = repo;
        }

        // ------------ CREATE-OPERATIONS----------------------

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

        [HttpPost("/seasons/{id}/drivers/create")]
        public async Task<Driver[]> CreateDrivers(Guid id, [FromBody] ModifiedDriver[] drivers)
        {
            List<Driver> driverList = new List<Driver>();

            foreach (ModifiedDriver driver in drivers)
            {
                Driver tempDriver = new Driver();
                tempDriver.Id = Guid.NewGuid();
                tempDriver.Name = driver.Name;
                tempDriver.Nationality = driver.Nationality;
                tempDriver.Team = driver.Team;
                driverList.Add(tempDriver);
            }

            return await _repo.CreateDrivers(id, driverList);

        }

        [HttpPost("/seasons/{id}/event/create")]
        public async Task<Event> CreateEvent(Guid id, [FromBody] ModifiedEvent modifiedEvent)
        {
            Event createdEvent = new Event()
            {
                Id = Guid.NewGuid(),
                Name = modifiedEvent.Name,
                Date = modifiedEvent.Date,
                Track = _repo.CreateTrack(modifiedEvent.Track),
                Standings = new List<KeyValuePair<Driver, int>>()
            };

            return await _repo.CreateEvent(id, createdEvent);
        }

        // ------------ READ-OPERATIONS----------------------

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

        [HttpGet("/seasons/get/{id}/drivers/{driverId}")]
        public async Task<Driver> GetDriver(Guid id, Guid driverId)
        {
            return await _repo.GetDriver(id, driverId);
        }

        [HttpGet("/seasons/get/{id}/drivers")]
        public async Task<Driver[]> GetAllDrivers(Guid id)
        {
            return await _repo.GetAllDrivers(id);
        }

        [HttpGet("/seasons/get/{seasonId}/index")]
        public async Task<Int32> GetCurrentEventIndex(Guid seasonId)
        {
            return await _repo.GetCurrentEventIndex(seasonId);
        }

        [HttpGet("/seasons/get/{seasonId}/event/{eventId}")]
        public async Task<Event> GetSeasonEvent(Guid seasonId, Guid eventId)
        {
            return await _repo.GetSeasonEvent(seasonId, eventId);
        }

        [HttpGet("/seasons/get/{seasonId}/event/{eventId}/track")]
        public async Task<Track> GetSeasonTrack(Guid id, Guid eventId)
        {
            return await _repo.GetSeasonTrack(id, eventId);
        }

        // ------------ UPDATE-OPERATIONS----------------------

        [HttpPut("/seasons/modify/{id}/index")]
        public async Task<Season> ModifySeasonIndex(Guid id, [FromQuery] int index)
        {
            return await _repo.ModifySeasonIndex(id, index);
        }

        [HttpPut("/seasons/modify/{seasonId}/event/{eventId}")]
        public async Task<Event> ModifySeasonEvent(Guid seasonId, Guid eventId, [FromBody] ModifiedEvent modifiedEvent)
        {
            Event eventToModify = await GetSeasonEvent(seasonId, eventId);

            eventToModify.Name = modifiedEvent.Name;
            eventToModify.Date = modifiedEvent.Date;

            eventToModify.Track.Name = modifiedEvent.Track.Name;
            eventToModify.Track.Country = modifiedEvent.Track.Country;

            return await _repo.ModifySeasonEvent(seasonId, eventId, eventToModify);
        }

        [HttpPut("/seasons/modify/{seasonId}/driver/{driverId}")]
        public async Task<Driver> ModifyDriver(Guid seasonId, Guid driverId, [FromBody] ModifiedDriver modifiedDriver)
        {
            Driver driverToModify = await GetDriver(seasonId, driverId);

            driverToModify.Name = modifiedDriver.Name;
            driverToModify.Nationality = modifiedDriver.Nationality;
            driverToModify.Team = modifiedDriver.Team;

            return await _repo.ModifyDriver(seasonId, driverId, driverToModify);
        }

        [HttpPut("/seasons/modify/{seasonId}/event/{eventId}/track/{trackId}")]
        public async Task<Track> ModifyTrack(Guid seasonId, Guid eventId, Guid trackId, [FromBody] ModifiedTrack modifiedTrack)
        {
            Track trackToModify = await GetSeasonTrack(seasonId, eventId);

            trackToModify.Name = modifiedTrack.Name;
            trackToModify.Country = modifiedTrack.Country;

            return await _repo.ModifyTrack(seasonId, eventId, trackToModify);
        }

        // ------------ DELETE-OPERATIONS----------------------

        [HttpDelete("/seasons/delete/{id}")]
        public async Task<Season> DeleteSeason(Guid id)
        {
            return await _repo.DeleteSeason(id);
        }
    }
}