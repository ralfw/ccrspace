using System;
using CcrSpaces.Channels;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testCcrSpaceExtension : TestFixtureBase
    {
        [Test]
        public void Create_oneway_port()
        {
            mocks.ReplayAll();

            var mockCf = new MockChannelFactory {POneWay = new Port<int>()};
            Action<int> handler = n => { };

            CcrsChannelFactory.Instance = mockCf;

            Assert.AreSame(mockCf.POneWay, base.mockSpace.CreateChannel(handler));

            Assert.AreSame(handler, mockCf.CfgOneWay.MessageHandler);
            Assert.AreSame(dpq, mockCf.CfgOneWay.TaskQueue);
            Assert.AreEqual(CcrsHandlerModes.Sequential, mockCf.CfgOneWay.HandlerMode);
        }


        [Test]
        public void Create_reqresp_port()
        {
            mocks.ReplayAll();

            var mockCf = new MockChannelFactory
                             {
                                 PReqResp = new PortSet<string, CcrsRequest<string, int>>()
                             };
            Action<string, Port<int>> reqHandler = (s, pi) => { };
            Action<int> respHandler = n => { };

            CcrsChannelFactory.Instance = mockCf;

            Assert.AreSame(mockCf.PReqResp, mockSpace.CreateChannel(reqHandler, respHandler));

            Assert.AreSame(reqHandler, mockCf.CgfReqResp.InputMessageHandler);
            Assert.AreSame(respHandler, mockCf.CgfReqResp.OutputMessageHandler);
            Assert.AreSame(base.dpq, mockCf.CgfReqResp.TaskQueue);
            Assert.AreEqual(CcrsHandlerModes.Sequential, mockCf.CgfReqResp.InputHandlerMode);
            Assert.AreEqual(CcrsHandlerModes.Sequential, mockCf.CgfReqResp.OutputHandlerMode);
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


        [Test]
        public void Create_filter()
        {
            mocks.ReplayAll();

            //TODO: base test on mocks
            var f = base.mockSpace.CreateChannel<string, int>(
                        s => s.Length, 
                        base.mockSpace.CreateChannel<int>(n => base.are.Set()));

            f.Post("hello");

            Assert.IsTrue(base.are.WaitOne(500));
        }
    }


    public class MockChannelFactory : ICcrsChannelFactory
    {
        public Port<int> POneWay;
        public CcrsOneWayChannelConfig<int> CfgOneWay;

        public PortSet<string, CcrsRequest<string, int>> PReqResp;
        public CcrsRequestResponseChannelConfig<string, int> CgfReqResp;

        #region Implementation of ICcrsChannelFactory

        public Port<T> CreateChannel<T>(CcrsOneWayChannelConfig<T> config)
        {
            this.CfgOneWay = config as CcrsOneWayChannelConfig<int>;
            return this.POneWay as Port<T>;
        }

        public PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(CcrsRequestResponseChannelConfig<TInput, TOutput> config)
        {
            this.CgfReqResp = config as CcrsRequestResponseChannelConfig<string, int>;
            return this.PReqResp as PortSet<TInput, CcrsRequest<TInput, TOutput>>;
        }

        public Port<TInput> CreateChannel<TInput, TOutput>(CcrsFilterChannelConfig<TInput, TOutput> config)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
