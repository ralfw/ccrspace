using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using CcrSpaces.Core;
using Microsoft.Ccr.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testChannelFactoryExtension
    {
        [Test]
        public void Create_port()
        {
            MockRepository mocks = new MockRepository();

            ICcrSpace space = mocks.Stub<ICcrSpace>();
            DispatcherQueue dpq = new DispatcherQueue();
            space.Expect(x => x.DefaultTaskQueue).Return(dpq);

            mocks.ReplayAll();

            var mockCf = new MockChannelFactory();
            mockCf.P = new Port<int>();
            Action<int> handler = n => { };

            ChannelFactory.Instance = mockCf;

            Assert.AreSame(mockCf.P, space.CreateChannel<int>(handler));

            Assert.AreSame(handler, mockCf.Cfg.MessageHandler);
            Assert.AreSame(dpq, mockCf.Cfg.TaskQueue);
            Assert.AreEqual(CcrsChannelHandlerModes.Sequential, mockCf.Cfg.HandlerMode);
        }
    }


    public class MockChannelFactory : IChannelFactory
    {
        public Port<int> P;
        public CcrsChannelConfig<int> Cfg;

        #region Implementation of IChannelFactory

        public Port<T> CreateChannel<T>(CcrsChannelConfig<T> config)
        {
            this.Cfg = config as CcrsChannelConfig<int>;
            return this.P as Port<T>;
        }

        #endregion
    }
}
