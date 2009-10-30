using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Flows;
using GeneralTestInfrastructure;
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
        public void Multi_stage_flow_with_terminal_stage()
        {
            var fsi = new CcrsFlow<string, int>((s, pn) => pn.Post(s.Length));
            var sut = sut.Continue<int>(n => base.are.Set());

            sut.Post("hello");

            Assert.IsTrue(base.are.WaitOne(1000));
        }
    }
}
