using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Channels.Extensions;
using CcrSpaces.Core.Core;
using CcrSpaces.Core.Flows;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Flows
{
    [TestFixture]
    public class testFlows : TestFixtureBase
    {
        [Test]
        public void Single_stage_flow_with_terminal_stage()
        {
            var sut = new CcrsFlow<string>(s => base.are.Set());

            sut.Post("hello");

            Assert.IsTrue(base.are.WaitOne(1000));
        }


        [Test]
        public void Two_stage_flow_with_terminal_stage()
        {
            var fsi = new CcrsFlow<string, int>((s, pn) => pn.Post(s.Length));
            var sut = fsi.Terminate(n => base.are.Set());

            sut.Post("hello");

            Assert.IsTrue(base.are.WaitOne(1000));
        }


        [Test]
        public void Multi_stage_flow_with_terminal_stage()
        {
            var fsi = new CcrsFlow<string, int>((s, pn) => pn.Post(s.Length));
            var fib = fsi.Continue<bool>((n, pb) => pb.Post(n%2 == 0));
            var sut = fib.Terminate(b =>base.are.Set());

            sut.Post("hello");

            Assert.IsTrue(base.are.WaitOne(1000));
        }


        [Test]
        public void Multi_stage_flow_without_terminal_stage()
        {
            var fsi = new CcrsFlow<string, int>((s, pn) => pn.Post(s.Length));
            var sut = fsi.Continue<bool>((n, pb) => pb.Post(n%2==0));

            var pResult = new Port<bool>();
            pResult.RegisterGenericSyncReceiver(o => base.are.Set());

            sut.Post(new CcrsRequest<string,bool>("hello", pResult));

            Assert.IsTrue(base.are.WaitOne(1000));
        }
    }
}
