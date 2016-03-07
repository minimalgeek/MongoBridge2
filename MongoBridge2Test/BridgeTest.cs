using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmiBroker;
using MongoBridge2;

namespace MongoBridge2Test
{
    [TestClass]
    public class BridgeTest
    {
        private Bridge bridge;

        [TestInitialize]
        public void init()
        {
            bridge = new Bridge();
        }

        [TestMethod]
        public void TestMongoMA(){
            var array = new ATArray();

            var ret = bridge.MongoMA(array, 0f);
        }
    }
}
