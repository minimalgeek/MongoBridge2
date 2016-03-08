using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmiBroker;
using MongoBridge2;
using MongoBridge2.Mongo;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoBridge2Test.Mongo
{
    [TestClass]
    public class EarningsCallDAOTest
    {
        private static int[] Words = new int[100];
        private static int[] PositiveCount = new int[100];

        private EarningsCallDAO dao;

        [TestInitialize]
        public void init()
        {
            dao = new EarningsCallDAO();
        }

        [TestMethod]
        public void TestDBQuery()
        {
            ICollection<BsonDocument> list = dao.findByTradingSymbolAndSortByPublishDate("ROST");

            DateTime firstDate = new DateTime(2016, 1, 1);
            DateTime lastDate = new DateTime(2016, 3, 8);

            DateTime dateIterator = firstDate;
            int idx = 0;

            // set null values at the beginning of the array
            while (!dateIterator.Equals(lastDate))
            {
                BsonDocument foundDoc = SearchMatchingEC(list, dateIterator);
                AddToAllAFL(foundDoc, idx);

                dateIterator = dateIterator.AddDays(1);
                idx++;
            }

            Console.WriteLine("Hello!");
        }

        private BsonDocument SearchMatchingEC(ICollection<BsonDocument> list, DateTime amiDt)
        {
            foreach (BsonDocument doc in list)
            {
                DateTime dt = (DateTime)doc.GetElement("publishDate").Value;
                if (dt.Date == amiDt.Date)
                {
                    return doc;
                }
            }

            return null;
        }

        private void AddToAllAFL(BsonDocument doc, int idx)
        {
            if (doc != null)
            {
                try
                {
                    Words[idx] = doc.GetElement("words").Value.AsBsonArray.Count;
                    PositiveCount[idx] = doc.GetElement("tone").Value.AsBsonDocument["positiveCount"].AsInt32;
                }
                catch (Exception e)
                {
                    AddToAllAFL(null, idx);
                }
            }
            else
            {
                Words[idx] = 0;
                PositiveCount[idx] = 0;
            }
        }
    }
}
