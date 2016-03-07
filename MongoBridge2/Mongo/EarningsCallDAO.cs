using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoBridge2.Mongo
{
    class EarningsCallDAO
    {
        private const string URL = "mongodb://localhost";
        private const string DATABASE = "insider";
        private const string EARNINGS_CALL = "earnings_call";

        protected IMongoClient _client;
        protected IMongoDatabase _database;
        protected IMongoCollection<BsonDocument> _collection;

        public EarningsCallDAO()
        {
            _client = new MongoClient(URL);
            _database = _client.GetDatabase(DATABASE);
            _collection = _database.GetCollection<BsonDocument>(EARNINGS_CALL);
        }

        public ICollection<BsonDocument> findAll()
        {
            return _collection.Find(new BsonDocument()).ToList();
        }

        public ICollection<BsonDocument> findByTradingSymbolAndSortByPublishDate(string tradingSymbol)
        {
            var filter  = Builders<BsonDocument>.Filter.Eq("tradingSymbol", tradingSymbol);
            var sort    = Builders<BsonDocument>.Sort.Descending("publishDate");

            return _collection.Find(filter).Sort(sort).ToList();
        }
    }
}
