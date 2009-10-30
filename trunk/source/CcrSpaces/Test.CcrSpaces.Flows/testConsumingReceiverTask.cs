using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Flows.Infrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Flows
{
    [TestFixture]
    public class testConsumingReceiverTask
    {
        [Test]
        public void Post_causes_new_elements_to_be_processed_immediately()
        {
            var p = new Port<int>();

            int nReceived = 0;
            (p as IPortReceive).RegisterReceiver(new IPortReceiveExtensions.ConsumingReceiverTask(n => { nReceived = (int)n; }));

            p.Post(1);

            Assert.AreEqual(1, nReceived);
            Assert.IsFalse(p.Test(out nReceived));
        }


        [Test]
        public void Registers_sync_receiver_with_extension_method()
        {
            var p = new Port<int>();

            p.RegisterGenericSyncReceiver(x => {  });

            p.Post(1);

            int n;
            Assert.IsFalse(p.Test(out n));
        }
    }
}
