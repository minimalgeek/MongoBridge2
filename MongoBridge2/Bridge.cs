using System;
using AmiBroker;
using AmiBroker.PlugIn;
using AmiBroker.Utils;
using MongoBridge2.Mongo;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoBridge2
{
    public class Bridge : IndicatorBase
    {
        private static ATAfl WordsCount = new ATAfl("WordsCount");

        private static ATAfl PositiveCount = new ATAfl("PositiveCount");
        private static ATAfl NegativeCount = new ATAfl("NegativeCount");
        private static ATAfl StrongCount = new ATAfl("StrongCount");
        private static ATAfl WeakCount = new ATAfl("WeakCount");
        private static ATAfl ActiveCount = new ATAfl("ActiveCount");
        private static ATAfl PassiveCount = new ATAfl("PassiveCount");
        private static ATAfl OverstatedCount = new ATAfl("OverstatedCount");
        private static ATAfl UnderstatedCount = new ATAfl("UnderstatedCount");

        private static ATAfl HenryPositiveCount = new ATAfl("HenryPositiveCount");
        private static ATAfl HenryNegativeCount = new ATAfl("HenryNegativeCount");

        // Q and A

        private static ATAfl QAndAWordsCount = new ATAfl("QAndAWordsCount");

        private static ATAfl QAndAPositiveCount = new ATAfl("QAndAPositiveCount");
        private static ATAfl QAndANegativeCount = new ATAfl("QAndANegativeCount");
        private static ATAfl QAndAStrongCount = new ATAfl("QAndAStrongCount");
        private static ATAfl QAndAWeakCount = new ATAfl("QAndAWeakCount");
        private static ATAfl QAndAActiveCount = new ATAfl("QAndAActiveCount");
        private static ATAfl QAndAPassiveCount = new ATAfl("QAndAPassiveCount");
        private static ATAfl QAndAOverstatedCount = new ATAfl("QAndAOverstatedCount");
        private static ATAfl QAndAUnderstatedCount = new ATAfl("QAndAUnderstatedCount");

        private static ATAfl QAndAHenryPositiveCount = new ATAfl("QAndAHenryPositiveCount");
        private static ATAfl QAndAHenryNegativeCount = new ATAfl("QAndAHenryNegativeCount");

        private EarningsCallDAO dao;

        public Bridge()
            : base()
        {
            dao = new EarningsCallDAO();
        }

        [ABMethod]
        public ATArray MongoMA(ATArray array, float period)
        {
            ATArray result = AFAvg.Ma(array, period);
            return result;
        }

        [ABMethod]
        public void EarningsCallTone(string tradingSymbol)
        {
            try
            {
                ICollection<BsonDocument> list = dao.findByTradingSymbolAndSortByPublishDate(tradingSymbol);

                ATDateTime firstDate = this.DateAndTime[0];
                ATDateTime lastDate = this.DateAndTime[this.DateAndTime.Length - 1];

                ATDateTime dateIterator = lastDate;
                int idx = 0;

                // set null values at the beginning of the array
                while (!dateIterator.Equals(firstDate))
                {
                    BsonDocument foundDoc = SearchMatchingEC(list, dateIterator);
                    if (foundDoc != null)
                    {

                    }
                    else
                    {

                    }

                    dateIterator.Day = dateIterator.Day + 1;
                    idx++;
                }
            }
            catch (Exception e)
            {
                // present error message on indicator panel and the Log-Trace window
                YException.Show("Error while executing MongoBridge2.EarningsCallTone: ", e);
            }
        }

        private BsonDocument SearchMatchingEC(ICollection<BsonDocument> list, ATDateTime dateTime)
        {
            foreach (BsonDocument doc in list)
            {
                DateTime dt = (DateTime)doc.GetElement("publishDate").Value;
                if (dt.Equals(dateTime.Date))
                {
                    return doc;
                }
            }
            return null;
        }
    }
}
