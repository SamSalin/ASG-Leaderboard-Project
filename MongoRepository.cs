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

        public async Task<Season> CreateSeason(Season season)
        {
            await _seasonCollection.InsertOneAsync(season);
            return season;
        }

        public Task<Event> CreateEvent(Guid id, Event createdEvent)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var push = Builders<Season>.Update.Push("Events", createdEvent);
            _seasonCollection.FindOneAndUpdateAsync(filter, push);

            return Task.FromResult(createdEvent);
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
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var push = Builders<Season>.Update.PushEach("Drivers", driverList);
            await _seasonCollection.FindOneAndUpdateAsync(filter, push);

            //Every time we create new drivers, we should update the season standings
            await AddToSeasonStandings(id, driverList);
            await AddToEventStandings(id, driverList);

            return driverList.ToArray();
        }

        public async Task<Season> GetSeason(Guid id)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var season = await _seasonCollection.Find(filter).FirstAsync();
            return season;
        }

        public async Task<Season[]> GetAllSeasons()
        {
            var seasons = await _seasonCollection.Find(new BsonDocument()).ToListAsync();
            return seasons.ToArray();
        }

        public async Task<Event> GetSeasonEvent(Guid seasonId, Guid eventId)
        {
            var filter = Builders<Season>.Filter.ElemMatch<Event>(s => s.Events, e => e.Id == eventId);
            var search = await _seasonCollection.Find(filter).FirstAsync();

            return search.Events.FirstOrDefault(e => e.Id == eventId);
        }

        public async Task<Int32> GetCurrentEventIndex(Guid id)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var season = await _seasonCollection.Find(filter).FirstAsync();
            return season.CurrentEventIndex;
        }

        public async Task<Event> ModifySeasonEvent(Guid seasonid, Guid eventId, Event modifiedEvent)
        {
            var filter = Builders<Season>.Filter.Where(s => s.Id == seasonid && s.Events.Any(e => e.Id == eventId));
            var replace = Builders<Season>.Update.Set(e => e.Events[-1], modifiedEvent);

            await _seasonCollection.UpdateOneAsync(filter, replace);

            return modifiedEvent;
        }

        public async Task<Season> ModifySeasonIndex(Guid id, int index)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var update = Builders<Season>.Update.Set("CurrentEventIndex", index);

            return await _seasonCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Season> AddToSeasonStandings(Guid id, List<Driver> driverList)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var search = await _seasonCollection.Find(filter).FirstAsync();
            List<KeyValuePair<Driver, int>> standings = search.Standings;

            for (int i = 0; i < driverList.Count; i++)
            {
                standings.Add(new KeyValuePair<Driver, int>(driverList[i], default));
            }

            var replacement = Builders<Season>.Update.Set("Standings", standings);
            await _seasonCollection.FindOneAndUpdateAsync(filter, replacement);

            return await _seasonCollection.FindOneAndUpdateAsync(filter, replacement, options);
        }

        public async Task<Season> AddToEventStandings(Guid id, List<Driver> driverList)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var search = await _seasonCollection.Find(filter).FirstAsync();

            for (int j = 0; j < search.Events.Count; j++)
            {
                for (int i = 0; i < driverList.Count; i++)
                {
                    search.Events[j].Standings.Add(new KeyValuePair<Driver, int>(driverList[i], default));
                }
            }

            var replacement = Builders<Season>.Update.Set("Events", search.Events);
            await _seasonCollection.FindOneAndUpdateAsync(filter, replacement);

            return await _seasonCollection.FindOneAndUpdateAsync(filter, replacement, options);
        }

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
            int lastEventIndex = await GetCurrentEventIndex(id) - 1;
            if (lastEventIndex < 2)
            {
                //tämä estää errorin tässä vaiheessa
                lastEventIndex = 0;


                //error tähän, ei aiempia kilpailuita
                //return null;
            }

            var season = await GetSeason(id);

            List<KeyValuePair<Driver, int>> standings = season.Events[lastEventIndex].Standings;

            Console.WriteLine("Last event: " + season.Events[lastEventIndex].Name + " Date: " + season.Events[lastEventIndex].Date);
            for (int i = 0; i < season.Standings.Count; i++)
            {
                sija = 1 + i;
                Console.WriteLine(sija + ". " + standings[i].Key.Name + " , Points: " + standings[i].Value);
            }


            return null;
        }
        public async Task<string> SimulateNextEvent(Guid id)
        {
            Season season = await GetSeason(id);
            List<KeyValuePair<Driver, int>> updatedStandings = CalculateResults(season.Drivers);
            Console.WriteLine(season.Events[0].Standings[0].Value);

            for (int i = 0; i < updatedStandings.Count; i++)
            {
                Console.WriteLine("Driver: " + updatedStandings[i].Key.Name + ", Points: " + updatedStandings[i].Value);
            }

            return null;
        }

        public List<KeyValuePair<Driver, int>> CalculateResults(List<Driver> drivers)
        {
            Int32 points;
            Int32[] pointsForPlacements = new Int32[] { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };
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
    }
}