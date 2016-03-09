using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoBridge2.Mongo
{
    public class GenericDAO
    {
        protected IMongoClient _client;
        protected IMongoDatabase _database;
        protected IMongoCollection<BsonDocument> _collection;

        public GenericDAO(string url, string database, string collection)
        {
            _client = new MongoClient(url);
            _database = _client.GetDatabase(database);
            _collection = _database.GetCollection<BsonDocument>(collection);
        }

        public ICollection<BsonDocument> findAll()
        {
            return _collection.Find(new BsonDocument()).ToList();
        }

        public ICollection<BsonDocument> findFiltered(string filterExpression)
        {
            char[] separatingChars = { ';' };
            char[] inExpSeparatingChars = { '=' };
            string[] expressions = filterExpression.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            FilterDefinition<BsonDocument> filter = FilterDefinition<BsonDocument>.Empty;

            if (expressions.Length > 0)
            {
                foreach (string expression in expressions)
                {
                    string[] leftAndRightPart = expression.Split(inExpSeparatingChars, System.StringSplitOptions.RemoveEmptyEntries);
                    filter = filter & Builders<BsonDocument>.Filter.Eq(leftAndRightPart[0], leftAndRightPart[1]);
                }
            }

            return _collection.Find(filter).ToList();
        }
    }
}
