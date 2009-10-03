using System;
using System.Threading;
using CcrSpaces.Api;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testCcrsOneWayChannel
    {
        private AutoResetEvent are;

        [SetUp]
        public void Arrange()
        {
            this.are = new AutoResetEvent(false);
        }


        [Test]
        public void Standalone_creation()
        {
            var sut = new CcrsOneWayChannel<int>(n => this.are.Set());

            sut.Post(1);
            
            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Standalone_configuration()
        {
            var cfg = new CcrsOneWayChannelConfig<int> {MessageHandler = n=>this.are.Set(), TaskQueue=new DispatcherQueue()};
            var sut = new CcrsOneWayChannel<int>(cfg);

            sut.Post(1);

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Fluent_configuration()
        {
            var handler = new Action<int>(n => this.are.Set());

            var fluent = new CcrsOneWayChannelFluent<int>()
                .ProcessWith(handler)
                .Sequentially();

            CcrsOneWayChannelConfig<int> cfg = fluent;
            Assert.AreSame(handler, cfg.MessageHandler);
            Assert.IsTrue(cfg.ProcessSequentially);
        }
    }
}
