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
        private static ATAfl WordsCount = Init("WordsCount");

        private static ATAfl PositiveCount = Init("PositiveCount");
        private static ATAfl NegativeCount = Init("NegativeCount");
        private static ATAfl StrongCount = Init("StrongCount");
        private static ATAfl WeakCount = Init("WeakCount");
        private static ATAfl ActiveCount = Init("ActiveCount");
        private static ATAfl PassiveCount = Init("PassiveCount");
        private static ATAfl OverstatedCount = Init("OverstatedCount");
        private static ATAfl UnderstatedCount = Init("UnderstatedCount");

        private static ATAfl HenryPositiveCount = Init("HenryPositiveCount");
        private static ATAfl HenryNegativeCount = Init("HenryNegativeCount");

        // Q and A

        private static ATAfl QAndAWordsCount = Init("QAndAWordsCount");

        private static ATAfl QAndAPositiveCount = Init("QAndAPositiveCount");
        private static ATAfl QAndANegativeCount = Init("QAndANegativeCount");
        private static ATAfl QAndAStrongCount = Init("QAndAStrongCount");
        private static ATAfl QAndAWeakCount = Init("QAndAWeakCount");
        private static ATAfl QAndAActiveCount = Init("QAndAActiveCount");
        private static ATAfl QAndAPassiveCount = Init("QAndAPassiveCount");
        private static ATAfl QAndAOverstatedCount = Init("QAndAOverstatedCount");
        private static ATAfl QAndAUnderstatedCount = Init("QAndAUnderstatedCount");

        private static ATAfl QAndAHenryPositiveCount = Init("QAndAHenryPositiveCount");
        private static ATAfl QAndAHenryNegativeCount = Init("QAndAHenryNegativeCount");

        private static ATAfl Init(string name)
        {
            ATAfl ata = new ATAfl(name);
            ata.Set(new ATArray(ATFloat.Null));
            return ata;
        }

        // non-static

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
        public void TraceTest()
        {
            YTrace.Trace("TraceTest: Information", YTrace.TraceLevel.Information);
         }

        [ABMethod]
        public void EarningsCallTone(string tradingSymbol)
        {
            YTrace.Trace("Begin ECT method", YTrace.TraceLevel.Information);
            try
            {
                YTrace.Trace("Get all document for: " + tradingSymbol, YTrace.TraceLevel.Information);
                ICollection<BsonDocument> list = dao.findByTradingSymbolAndSortByPublishDate(tradingSymbol);
                //ICollection<BsonDocument> list = new List<BsonDocument>();

                ATDateTime first = this.DateAndTime[0];
                ATDateTime last = this.DateAndTime[this.DateAndTime.Length - 1];

                DateTime firstDate = new DateTime(first.Year, first.Month, first.Day);
                YTrace.Trace("First date: " + firstDate.ToString(), YTrace.TraceLevel.Information);
                DateTime lastDate = new DateTime(last.Year, last.Month, last.Day);
                YTrace.Trace("Last date: " + lastDate.ToString(), YTrace.TraceLevel.Information);

                DateTime dateIterator = firstDate;
                int idx = 0;

                // set null values at the beginning of the array
                while (!dateIterator.Equals(lastDate))
                {
                    YTrace.Trace("Check date: " + dateIterator.ToString(), YTrace.TraceLevel.Information);
                    
                    BsonDocument foundDoc = SearchMatchingEC(list, dateIterator);
                    AddToAllAFL(foundDoc, idx);

                    dateIterator = dateIterator.AddDays(1);
                    idx++;
                }
            }
            catch (Exception e)
            {
                YTrace.Trace("Error: " + e.ToString(), YTrace.TraceLevel.Error);
                YException.Show("Error while executing MongoBridge2.EarningsCallTone: ", e);
            }
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
                YTrace.Trace("Found doc: " + doc.ToString(), YTrace.TraceLevel.Information);
                try
                {
                    WordsCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("words").Value.AsBsonArray.Count;
                    
                    PositiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["positiveCount"].AsInt32;
                    NegativeCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["negativeCount"].AsInt32;
                    StrongCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["strongCount"].AsInt32;
                    WeakCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["weakCount"].AsInt32;
                    ActiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["activeCount"].AsInt32;
                    PassiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["passiveCount"].AsInt32;
                    OverstatedCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["overstatedCount"].AsInt32;
                    UnderstatedCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("tone").Value.AsBsonDocument["understatedCount"].AsInt32;

                    HenryPositiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("h_tone").Value.AsBsonDocument["positiveCount"].AsInt32;
                    HenryNegativeCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("h_tone").Value.AsBsonDocument["negativeCount"].AsInt32;

                    QAndAWordsCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("qAndAWords").Value.AsBsonArray.Count;

                    QAndAPositiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["positiveCount"].AsInt32;
                    QAndANegativeCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["negativeCount"].AsInt32;
                    QAndAStrongCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["strongCount"].AsInt32;
                    QAndAWeakCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["weakCount"].AsInt32;
                    QAndAActiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["activeCount"].AsInt32;
                    QAndAPassiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["passiveCount"].AsInt32;
                    QAndAOverstatedCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["overstatedCount"].AsInt32;
                    QAndAUnderstatedCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_tone").Value.AsBsonDocument["understatedCount"].AsInt32;

                    QAndAHenryPositiveCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_h_tone").Value.AsBsonDocument["positiveCount"].AsInt32;
                    QAndAHenryNegativeCount.GetArray(ATFloat.Null, true)[idx] = doc.GetElement("q_and_a_h_tone").Value.AsBsonDocument["negativeCount"].AsInt32;
                }
                catch (Exception e)
                {
                    YTrace.Trace("======> ERROR: " + e.ToString(), YTrace.TraceLevel.Error);
                }
            }
        }
    }
}
