using System;
using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Core;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Channels.Extensions;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testChannelFactoryFilter : TestFixtureBase
    {
        private CcrsChannelFactory sut;

        protected override void FixtureArrange()
        {
            this.sut = new CcrsChannelFactory();
        }


        [Test]
        public void Create_filter_channel()
        {
            var pResponses = new Port<int>();
            pResponses.WireUpHandler(null, true, n => base.are.Set());

            var cfg = new CcrsFilterChannelConfig<string, int>
                          {
                              InputMessageHandler = (s, pn) => pn.Post(s.Length),
                              OutputPort = pResponses
                          };
            var p = this.sut.CreateChannel(cfg);

            p.Post("hello");

            Assert.IsTrue(base.are.WaitOne(500));
        }
    }
}
