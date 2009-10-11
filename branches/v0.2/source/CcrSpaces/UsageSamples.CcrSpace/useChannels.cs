using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useChannels
    {
        [Test]
        public void Create_oneway_channel()
        {
            using(var space = new CcrSpace())
            {
                // all channels are served by threads from the CCR space
                // default thread pool

                // sequential channel
                // it´s persistent, ie. it processes all messages sent to it
                Port<int> p;
                p = space.CreateChannel<int>(Console.WriteLine);

                // parallel channel
                p = space.CreateChannel<int>(Console.WriteLine, 
                                             CcrsChannelHandlerModes.Parallel);

                // a channel processing messages in the sync context
                // of its creator. use this to access GUI controls during message processing
                p = space.CreateChannel<int>(Console.WriteLine,
                                             CcrsChannelHandlerModes.InCreatorSyncContext);
            }
        }
    }
}
