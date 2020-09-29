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

        internal Task<Driver[]> CreateDrivers(Guid id, List<Driver> driverList)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            var push = Builders<Season>.Update.PushEach("Drivers", driverList);
            _seasonCollection.FindOneAndUpdateAsync(filter, push);

            //Every time we create new drivers, we should update the season standings

            return Task.FromResult(driverList.ToArray());
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

        public async Task<Season> DeleteSeason(Guid id)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, id);
            Season deletedSeason = await _seasonCollection.Find(filter).FirstAsync();

            await _seasonCollection.FindOneAndDeleteAsync(filter);
            return deletedSeason;
        }
    }
}