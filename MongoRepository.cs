using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ASG_Leaderboard_Project
{
    public class MongoRepository
    {
        private readonly IMongoCollection<Season> _seasonCollection;
        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection;
        private static Random rng = new Random();

        FindOneAndUpdateOptions<Season> options = new FindOneAndUpdateOptions<Season>()
        {
            ReturnDocument = ReturnDocument.After
        };

        public MongoRepository()
        {
            /*
            For local testing:
            1. Create database called asg
            2. Create collection called seasons
            */

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("asg");
            _seasonCollection = database.GetCollection<Season>("seasons");
            _bsonDocumentCollection = database.GetCollection<BsonDocument>("seasons");
        }

        //------------------ CRUD-OPERATIONS-----------------------


        //----CREATE-----------------------
        public async Task<Season> CreateSeason(Season season)
        {
            await _seasonCollection.InsertOneAsync(season);
            return season;
        }

        public async Task<Event> CreateEvent(Guid id, Event createdEvent)
        {
            var season = await GetSeason(id);

            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var push = Builders<Season>.Update.Push("Events", createdEvent);
            await _seasonCollection.FindOneAndUpdateAsync(filter, push);

            return createdEvent;
        }
        public Track CreateTrack(ModifiedTrack modifiedTrack)
        {
            Track track = new Track()
            {
                Id = Guid.NewGuid(),
                Name = modifiedTrack.Name,
                Country = modifiedTrack.Country
            };

            return track;
        }

        public async Task<Driver[]> CreateDrivers(Guid id, List<Driver> driverList)
        {
            var season = await GetSeason(id);

            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var push = Builders<Season>.Update.PushEach("Drivers", driverList);
            await _seasonCollection.FindOneAndUpdateAsync(filter, push);

            //Every time we create new drivers, we should add them to the season standings
            await AddToSeasonStandings(id, driverList);

            return driverList.ToArray();
        }

        //----READ-----------------------

        public async Task<Season> GetSeason(Guid id)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var season = await _seasonCollection.Find(filter).FirstOrDefaultAsync();

            if (season == null)
            {
                throw new NotFoundException("Season not found!");
            }

            return season;
        }

        public async Task<Season[]> GetAllSeasons()
        {
            var seasons = await _seasonCollection.Find(new BsonDocument()).ToListAsync();

            if (!seasons.Any())
            {
                throw new NotFoundException("No seasons found!");
            }

            return seasons.ToArray();
        }

        public async Task<Driver> GetDriver(Guid id, Guid driverId)
        {
            var filter = Builders<Season>.Filter.ElemMatch<Driver>(s => s.Drivers, d => d.Id == driverId);
            var search = await _seasonCollection.Find(filter).FirstAsync();

            return search.Drivers.FirstOrDefault(d => d.Id == driverId);
        }

        public async Task<Driver[]> GetAllDrivers(Guid id)
        {
            var season = await GetSeason(id);

            return season.Drivers.ToArray();
        }

        public async Task<Track> GetSeasonTrack(Guid id, Guid eventId)
        {
            var filter = Builders<Season>.Filter.ElemMatch<Event>(s => s.Events, e => e.Id == eventId);
            var search = await _seasonCollection.Find(filter).FirstAsync();

            return search.Events.Single(e => e.Id == eventId).Track;

        }

        public async Task<Event> GetSeasonEvent(Guid seasonId, Guid eventId)
        {
            var filter = Builders<Season>.Filter.ElemMatch<Event>(s => s.Events, e => e.Id == eventId);
            var search = await _seasonCollection.Find(filter).FirstAsync();

            return search.Events.FirstOrDefault(e => e.Id == eventId);
        }

        public async Task<int> GetCurrentEventIndex(Guid id)
        {
            var season = await GetSeason(id);
            return season.CurrentEventIndex;
        }

        //----UPDATE-----------------------

        public async Task<Event> ModifySeasonEvent(Guid seasonId, Guid eventId, Event modifiedEvent)
        {
            await CheckSeasonStartedException(seasonId);

            var filter = Builders<Season>.Filter.Where(s => s.Id == seasonId && s.Events.Any(e => e.Id == eventId));
            var update = Builders<Season>.Update.Set(e => e.Events[-1], modifiedEvent);

            await _seasonCollection.UpdateOneAsync(filter, update);

            return modifiedEvent;
        }

        public async Task<Season> ModifySeasonIndex(Guid id, int index)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var update = Builders<Season>.Update.Set("CurrentEventIndex", index);

            return await _seasonCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Driver> ModifyDriver(Guid seasonId, Guid driverId, Driver modifiedDriver)
        {
            await CheckSeasonStartedException(seasonId);

            var filter = Builders<Season>.Filter.Where(s => s.Id == seasonId && s.Drivers.Any(d => d.Id == driverId));
            var update = Builders<Season>.Update.Set(s => s.Drivers[-1], modifiedDriver);

            await _seasonCollection.UpdateOneAsync(filter, update);

            return modifiedDriver;
        }

        public async Task<Track> ModifyTrack(Guid seasonId, Guid eventId, Track modifiedTrack)
        {
            await CheckSeasonStartedException(seasonId);

            var filter = Builders<Season>.Filter.Where(s => s.Id == seasonId && s.Events.Any(e => e.Id == eventId));
            var update = Builders<Season>.Update.Set(e => e.Events[-1].Track, modifiedTrack);

            await _seasonCollection.UpdateOneAsync(filter, update);

            return modifiedTrack;
        }

        public async Task<Season> AddToSeasonStandings(Guid id, List<Driver> driverList)
        {
            var season = await GetSeason(id);

            List<KeyValuePair<Driver, int>> standings = season.Standings;

            for (int i = 0; i < driverList.Count; i++)
            {
                standings.Add(new KeyValuePair<Driver, int>(driverList[i], default));
            }

            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var replacement = Builders<Season>.Update.Set("Standings", standings);
            await _seasonCollection.FindOneAndUpdateAsync(filter, replacement);

            return await _seasonCollection.FindOneAndUpdateAsync(filter, replacement, options);
        }

        public async Task<Season> AddToEventStandings(Guid id, List<Driver> driverList)
        {
            var season = await GetSeason(id);

            for (int j = 0; j < season.Events.Count; j++)
            {
                for (int i = 0; i < driverList.Count; i++)
                {
                    season.Events[j].Standings.Add(new KeyValuePair<Driver, int>(driverList[i], default));
                }
            }

            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var replacement = Builders<Season>.Update.Set("Events", season.Events);
            await _seasonCollection.FindOneAndUpdateAsync(filter, replacement);

            return await _seasonCollection.FindOneAndUpdateAsync(filter, replacement, options);
        }

        //----DELETE-----------------------

        public async Task<Season> DeleteSeason(Guid id)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            Season deletedSeason = await _seasonCollection.Find(filter).FirstAsync();

            await _seasonCollection.FindOneAndDeleteAsync(filter);
            return deletedSeason;
        }

        //-----------------Simulation----------------

        public async Task<List<string>> GetSeasonStandings(Guid seasonId)
        {
            var season = await GetSeason(seasonId);
            var standingsList = season.Standings;
            List<string> finalList = new List<string>();

            standingsList.Sort((x, y) => x.Value.CompareTo(y.Value));
            standingsList.Reverse();

            foreach (var item in standingsList)
            {
                string tempString = item.Key.Name + ": " + item.Value.ToString();
                finalList.Add(tempString);
            }

            return finalList;
        }

        public async Task<List<string>> GetEventStandings(Guid seasonId, Guid eventId)
        {
            var tempEvent = await GetSeasonEvent(seasonId, eventId);
            var standingsList = tempEvent.Standings;
            List<string> finalList = new List<string>();

            standingsList.Sort((x, y) => x.Value.CompareTo(y.Value));
            standingsList.Reverse();

            foreach (var item in standingsList)
            {
                string tempString = item.Key.Name + ": " + item.Value.ToString();
                finalList.Add(tempString);
            }

            return finalList;
        }

        public async Task<string> LastEvent(Guid id)
        {
            int sija = 0;
            string list = "";
            int lastEventIndex = await GetCurrentEventIndex(id) - 1;
            if (lastEventIndex < 1)
            {
                //error tähän, ei aiempia kilpailuita
                throw new NotFoundException("Last Event not found!");
            }

            var season = await GetSeason(id);

            List<KeyValuePair<Driver, int>> standings = season.Events[lastEventIndex].Standings;

            list += "Last event: " + season.Events[lastEventIndex].Name + " Date: " + season.Events[lastEventIndex].Date.ToString() + "\n";
            for (int i = 0; i < season.Standings.Count; i++)
            {
                sija = 1 + i;
                list += "\n" + sija.ToString() + ". " + standings[i].Key.Name + ", Points: " + standings[i].Value.ToString();
            }


            return list;
        }

        public async Task<string> NextEvent(Guid seasonId)
        {
            Season tmpSeason = await GetSeason(seasonId);
            int nextEventIndex = await GetCurrentEventIndex(seasonId);
            string tempString;
            if (nextEventIndex < tmpSeason.Events.Count)
            {
                var tmpEvent = tmpSeason.Events[nextEventIndex];
                tempString = tmpEvent.Name + "\nTrack: " + tmpEvent.Track.Name + "  " + tmpEvent.Track.Country + "\nDate: " + tmpEvent.Date.ToString();
            }
            else { throw new OutOfRangeException("Season has already ended!"); }

            return tempString;
        }

        public async Task<string> CompareDrivers(Guid seasonId, Guid driverId1, Guid driverId2)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetDriverStandings(Guid seasonId, Guid driverId)
        {
            var season = await GetSeason(seasonId);
            var driver = await GetDriver(seasonId, driverId);

            int simulatedEvents = season.CurrentEventIndex;
            int totalPoints = AddDriverStandings(season, driver);

            string returnString = "";

            returnString += "After " + simulatedEvents + " events, " + driver.Name + " has placed accordingly:\n";

            for (int i = 0; i < simulatedEvents; i++)
            {
                int index = season.Events[i].Standings.FindIndex(d => d.Key.Id == driver.Id) + 1;
                for (int j = 0; j < season.Events[i].Standings.Count; j++)
                {
                    if (season.Events[i].Standings[j].Key.Id == driverId)
                    {
                        returnString += "\n" + season.Events[i].Name + " - " + index;
                        break;
                    }
                }
            }

            returnString += "\n\nAccumulating " + totalPoints + " points in total";
            return returnString;
        }

        public async Task<string> SimulateNextEvent(Guid id)
        {
            Season season = await GetSeason(id);

            if (season.CurrentEventIndex == season.Events.Count)
            {
                throw new OutOfRangeException("Cannot simulate season that has already ended!");
            }

            //If the standings for the next event are empty, populate event standing with drivers from the season

            if (season.Events[season.CurrentEventIndex].Standings.Count == 0)
            {
                await AddToEventStandings(id, season.Drivers);
            }

            string returnString = "";
            returnString += ("Simulating event " + (season.CurrentEventIndex + 1) + ": " + season.Events[season.CurrentEventIndex].Name + "!\n");
            returnString += ("Results of the event were:\n");

            List<KeyValuePair<Driver, int>> updatedStandings = CalculateResults(season.Drivers);
            season.Events[season.CurrentEventIndex].Standings = updatedStandings;

            // For return print
            for (int i = 0; i < updatedStandings.Count; i++)
            {
                returnString += "\n" + (i + 1) + ". " + updatedStandings[i].Key.Name + " - " + updatedStandings[i].Value + " points";
            }

            // Update events in document
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);

            //Update CurrentLevelIndex in document
            season.CurrentEventIndex++;

            //Find every driver in season standings and calculate overall score by adding every event together

            for (int i = 0; i < season.Standings.Count; i++)
            {
                for (int j = 0; j < updatedStandings.Count; j++)
                {
                    if (updatedStandings[j].Key.Id == season.Standings[i].Key.Id)
                    {
                        season.Standings[i] = new KeyValuePair<Driver, int>(updatedStandings[j].Key, AddDriverStandings(season, updatedStandings[j].Key));
                    }
                }
            }

            returnString += "\n\nAfter " + season.CurrentEventIndex + " event(s), the season standings are as follows:\n";

            season.Standings.Sort((x, y) => x.Value.CompareTo(y.Value));
            season.Standings.Reverse();

            for (int i = 0; i < season.Standings.Count; i++)
            {
                returnString += "\n" + (i + 1) + ". " + season.Standings[i].Key.Name + " - " + season.Standings[i].Value + " points";
            }

            // Update whole document
            await _seasonCollection.ReplaceOneAsync(filter, season);

            return returnString;
        }

        public async Task<string> SimulateRestofTheSeason(Guid id)
        {
            var season = await GetSeason(id);

            if (season.CurrentEventIndex == season.Events.Count)
            {
                throw new OutOfRangeException("No more events to simulate!");
            }

            string returnString = "Simulating all remaining events\n";

            for (int i = season.CurrentEventIndex; i < season.Events.Count; i++)
            {
                returnString += "\n\n" + await SimulateNextEvent(id);
            }

            return returnString;

        }

        public int AddDriverStandings(Season season, Driver driver)
        {
            int points = 0;

            for (int i = 0; i < season.CurrentEventIndex; i++)
            {
                for (int j = 0; j < season.Events[i].Standings.Count; j++)
                {
                    if (season.Events[i].Standings[j].Key.Id == driver.Id)
                    {
                        points += season.Events[i].Standings[j].Value;
                    }
                }
            }
            return points;
        }

        //Shuffles through the list of drivers in the event and scores them accordingly
        public List<KeyValuePair<Driver, int>> CalculateResults(List<Driver> drivers)
        {
            int points;
            int[] pointsForPlacements = new int[] { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };
            List<KeyValuePair<Driver, int>> updatedStandings = new List<KeyValuePair<Driver, int>>();
            Shuffle(drivers);

            for (int i = 0; i < drivers.Count; i++)
            {
                if (i < 9)
                {
                    points = pointsForPlacements[i];
                }
                else
                {
                    points = 0;
                }

                updatedStandings.Add(new KeyValuePair<Driver, int>(drivers[i], points));
            }

            return updatedStandings;
        }

        public static void Shuffle(List<Driver> drivers)
        {
            int n = drivers.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Driver value = drivers[k];
                drivers[k] = drivers[n];
                drivers[n] = value;
            }
        }

        //------------------ERROR CHECKING-----------------

        public async Task CheckSeasonStartedException(Guid seasonId)
        {
            if (await GetCurrentEventIndex(seasonId) > 0)
            {
                throw new SeasonStartedException("Cannot modify season in progress!");
            }
        }
    }
}