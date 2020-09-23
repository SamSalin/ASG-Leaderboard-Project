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

        // Season methods

        public async Task<Season> CreateSeason(Season season)
        {
            await _seasonCollection.InsertOneAsync(season);
            return season;
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

    }
}