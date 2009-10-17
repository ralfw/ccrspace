using System;
using System.Threading;
using CcrSpaces.Channels;
using CcrSpaces.Core;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testCcrSpaceExtension : TestFixtureBase
    {
        [Test]
        public void Create_oneway_port()
        {
            mocks.ReplayAll();

            var mockCf = new MockChannelFactory();
            mockCf.POneWay = new Port<int>();
            Action<int> handler = n => { };

            CcrsChannelFactory.Instance = mockCf;

            Assert.AreSame(mockCf.POneWay, base.mockSpace.CreateChannel(handler));

            Assert.AreSame(handler, mockCf.CfgOneWay.MessageHandler);
            Assert.AreSame(dpq, mockCf.CfgOneWay.TaskQueue);
            Assert.AreEqual(CcrsChannelHandlerModes.Sequential, mockCf.CfgOneWay.HandlerMode);
        }


        [Test]
        public void Create_reqresp_port()
        {
            mocks.ReplayAll();

            var mockCf = new MockChannelFactory();
            mockCf.PReqResp = new PortSet<string, CcrsRequest<string, int>>();
            Action<string, Port<int>> reqHandler = (s, pi) => { };
            Action<int> respHandler = n => { };

            CcrsChannelFactory.Instance = mockCf;

            Assert.AreSame(mockCf.PReqResp, mockSpace.CreateChannel(reqHandler, respHandler));

            Assert.AreSame(reqHandler, mockCf.CgfReqResp.InputMessageHandler);
            Assert.AreSame(respHandler, mockCf.CgfReqResp.OutputMessageHandler);
            Assert.AreSame(base.dpq, mockCf.CgfReqResp.TaskQueue);
            Assert.AreEqual(CcrsChannelHandlerModes.Sequential, mockCf.CgfReqResp.InputHandlerMode);
            Assert.AreEqual(CcrsChannelHandlerModes.Sequential, mockCf.CgfReqResp.OutputHandlerMode);
        }


        [Test]
        public void Request_receive()
        {
            mocks.ReplayAll();

            CcrsChannelFactory.Instance = null;

            var p = base.mockSpace.CreateChannel<string, int>(s => s.Length);

            p.Request("the").Receive(n => this.are.Set());

            Assert.IsTrue(this.are.WaitOne(500));
        }
    }


    public class MockChannelFactory : ICcrsChannelFactory
    {
        public Port<int> POneWay;
        public CcrsChannelConfig<int> CfgOneWay;

        public PortSet<string, CcrsRequest<string, int>> PReqResp;
        public CcrsChannelConfig<string, int> CgfReqResp;

        #region Implementation of ICcrsChannelFactory

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
