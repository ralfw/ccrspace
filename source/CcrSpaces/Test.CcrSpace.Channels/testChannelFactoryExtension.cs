using System;
using System.Threading;
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
        private AutoResetEvent are;

        [SetUp]
        public void GlobalArrange()
        {
            this.are = new AutoResetEvent(false);
        }


        [Test]
        public void Create_oneway_port()
        {
            MockRepository mocks = new MockRepository();

            ICcrSpace space = mocks.Stub<ICcrSpace>();
            DispatcherQueue dpq = new DispatcherQueue();
            space.Expect(x => x.DefaultTaskQueue).Return(dpq);

            mocks.ReplayAll();

            var mockCf = new MockChannelFactory();
            mockCf.POneWay = new Port<int>();
            Action<int> handler = n => { };

            ChannelFactory.Instance = mockCf;

            Assert.AreSame(mockCf.POneWay, space.CreateChannel(handler));

            Assert.AreSame(handler, mockCf.CfgOneWay.MessageHandler);
            Assert.AreSame(dpq, mockCf.CfgOneWay.TaskQueue);
            Assert.AreEqual(CcrsChannelHandlerModes.Sequential, mockCf.CfgOneWay.HandlerMode);
        }


        [Test]
        public void Create_reqresp_port()
        {
            MockRepository mocks = new MockRepository();

            ICcrSpace space = mocks.Stub<ICcrSpace>();
            DispatcherQueue dpq = new DispatcherQueue();
            space.Expect(x => x.DefaultTaskQueue).Return(dpq);

            mocks.ReplayAll();

            var mockCf = new MockChannelFactory();
            mockCf.PReqResp = new PortSet<string, CcrsRequest<string, int>>();
            Action<string, Port<int>> reqHandler = (s, pi) => { };
            Action<int> respHandler = n => { };

            ChannelFactory.Instance = mockCf;

            Assert.AreSame(mockCf.PReqResp, space.CreateChannel(reqHandler, respHandler));

            Assert.AreSame(reqHandler, mockCf.CgfReqResp.InputMessageHandler);
            Assert.AreSame(respHandler, mockCf.CgfReqResp.OutputMessageHandler);
            Assert.AreSame(dpq, mockCf.CgfReqResp.TaskQueue);
            Assert.AreEqual(CcrsChannelHandlerModes.Sequential, mockCf.CgfReqResp.InputHandlerMode);
            Assert.AreEqual(CcrsChannelHandlerModes.Sequential, mockCf.CgfReqResp.OutputHandlerMode);
        }


        [Test]
        public void Request_receive()
        {
            MockRepository mocks = new MockRepository();

            ICcrSpace space = mocks.Stub<ICcrSpace>();
            DispatcherQueue dpq = new DispatcherQueue();
            space.Expect(x => x.DefaultTaskQueue).Return(dpq);

            mocks.ReplayAll();

            ChannelFactory.Instance = null;

            var p = space.CreateChannel<string, int>(s => s.Length);

            p.Request("the").Receive(n => this.are.Set());

            Assert.IsTrue(this.are.WaitOne(500));
        }
    }


    public class MockChannelFactory : IChannelFactory
    {
        public Port<int> POneWay;
        public CcrsChannelConfig<int> CfgOneWay;

        public PortSet<string, CcrsRequest<string, int>> PReqResp;
        public CcrsChannelConfig<string, int> CgfReqResp;

        #region Implementation of IChannelFactory

        public Port<T> CreateChannel<T>(CcrsChannelConfig<T> config)
        {
            this.CfgOneWay = config as CcrsChannelConfig<int>;
            return this.POneWay as Port<T>;
        }

        public PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(CcrsChannelConfig<TInput, TOutput> config)
        {
            this.CgfReqResp = config as CcrsChannelConfig<string, int>;
            return this.PReqResp as PortSet<TInput, CcrsRequest<TInput, TOutput>>;
        }

        #endregion
    }
}
