using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using CcrSpaces.Core.Core;
using GeneralTestInfrastructure;
using NUnit.Framework;
using CcrSpaces.Core.Flows;

namespace Test.CcrSpaces.Flows
{
    [TestFixture]
    public class testCcrSpaceExtension : TestFixtureBase
    {
        [Test]
        public void Creates_flow()
        {
            using(var space = new CcrSpace())
            {
                var sut = space.StartFlow<string, int>(s => s.Length)
                            .Terminate(n => base.are.Set());

                sut.Post("hello");

                Assert.IsTrue(base.are.WaitOne(1000));
            }
        }
    }
}
