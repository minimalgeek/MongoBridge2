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
        private static GenericDAO dao;
        private static Dictionary<string, ATArray> dataArrays;

        [ABMethod]
        public void TraceTest()
        {
            YTrace.Trace("TraceTest: Information", YTrace.TraceLevel.Information);
        }

        [ABMethod]
        public void Connect(string url, string database, string collection)
        {
            YTrace.Trace("Connecting to: " + url + " - " + database + " - " + collection, YTrace.TraceLevel.Information);
            dao = new GenericDAO(url, database, collection);
            YTrace.Trace("Connected!", YTrace.TraceLevel.Information);
        }

        [ABMethod]
        public void MongoQueryToAFL(string filterExpression, string dateColumn, string projection)
        {
            YTrace.Trace("Begin MongoQueryToAFL method: " + filterExpression, YTrace.TraceLevel.Information);
            InitDataArrays(projection);
            try
            {
                ICollection<BsonDocument> list = null;
                try
                {
                    list = dao.findFiltered(filterExpression, dateColumn);
                }
                catch (Exception e)
                {
                    YTrace.Trace("Exception in DAO: " + e.ToString(), YTrace.TraceLevel.Error);
                }

                ATDateTime first = this.DateAndTime[0];
                YTrace.Trace("First date (Ami): " + first.ToString(), YTrace.TraceLevel.Information);
                ATDateTime last = this.DateAndTime[this.DateAndTime.Length - 1];
                YTrace.Trace("Last date (Ami): " + last.ToString(), YTrace.TraceLevel.Information);

                /*
                DateTime firstDate = new DateTime(first.Year, first.Month, first.Day);
                YTrace.Trace("First date: " + firstDate.ToString(), YTrace.TraceLevel.Information);
                DateTime lastDate = new DateTime(last.Year, last.Month, last.Day);
                YTrace.Trace("Last date: " + lastDate.ToString(), YTrace.TraceLevel.Information);
                */

                for (int idx = 0; idx < this.DateAndTime.Length; idx++)
                {
                    ATDateTime dateIterator = this.DateAndTime[idx];
                    YTrace.Trace("Check date: " + dateIterator.ToString(), YTrace.TraceLevel.Information);

                    BsonDocument foundDoc = SearchMatchingDocument(list, dateIterator, dateColumn);
                    AddToAFLs(foundDoc, idx);
                }
                InitAFLs();
            }
            catch (Exception e)
            {
                YTrace.Trace("Error: " + e.ToString(), YTrace.TraceLevel.Error);
                YException.Show("Error while executing MongoQueryToAFL: ", e);
            }
        }

        private BsonDocument SearchMatchingDocument(ICollection<BsonDocument> list, ATDateTime amiDt, string dateColumn)
        {
            DateTime dateTime = new DateTime(amiDt.Year, amiDt.Month, amiDt.Day);
            foreach (BsonDocument doc in list)
            {
                DateTime dt = (DateTime)doc.GetElement(dateColumn).Value;
                if (dt.Date == dateTime.Date)
                {
                    return doc;
                }
            }

            return null;
        }

        private void AddToAFLs(BsonDocument doc, int idx)
        {
            if (doc != null)
            {
                YTrace.Trace("Found doc: " + doc.ToString(), YTrace.TraceLevel.Information);
                char[] separatingChars = { '.' };
                try
                {
                    foreach (KeyValuePair<string, ATArray> entry in dataArrays)
                    {
                        string[] parts = entry.Key.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 1) {
                            entry.Value[idx] = GetAsFloat(doc.GetElement(parts[0]).Value);
                        } else if (parts.Length == 2) {
                            entry.Value[idx] = GetAsFloat(doc.GetElement(parts[0]).Value.AsBsonDocument[parts[1]]);
                        }
                    }
                }
                catch (Exception e)
                {
                    YTrace.Trace("======> ERROR: " + e.ToString(), YTrace.TraceLevel.Error);
                }
            }
        }

        private float GetAsFloat(BsonValue val)
        {
            if (val.IsInt32)
            {
                YTrace.Trace("Add <int> to list: " + val.ToString(), YTrace.TraceLevel.Information);
                return (float)val.AsInt32;
            }

            if (val.IsInt64)
            {
                YTrace.Trace("Add <long> to list: " + val.ToString(), YTrace.TraceLevel.Information);
                return (float)val.AsInt64;
            }

            if (val.IsDouble)
            {
                YTrace.Trace("Add <double> to list: " + val.ToString(), YTrace.TraceLevel.Information);
                return (float)val.AsDouble;
            }

            YTrace.Trace("Unprocessed type: " + val.BsonType.ToString(), YTrace.TraceLevel.Information);
            return ATFloat.Null;
        }

        private void InitDataArrays(string projection)
        {
            dataArrays = new Dictionary<string, ATArray>();

            char[] separatingChars = { ';' };
            string[] expressions = projection.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string expression in expressions)
            {
                dataArrays.Add(expression, new ATArray(ATFloat.Null));
            }
        }

        private void InitAFLs()
        {
            foreach (KeyValuePair<string, ATArray> entry in dataArrays)
            {
                string saveKey = entry.Key.Replace('.', '_');
                YTrace.Trace("ATAfl save to: " + saveKey, YTrace.TraceLevel.Information);
                ATAfl.SaveTo(saveKey, entry.Value);
            }
        }
    }
}
