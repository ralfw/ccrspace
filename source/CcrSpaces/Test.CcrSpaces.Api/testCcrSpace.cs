using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Api;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testCcrSpace
    {
        private AutoResetEvent are;

        [SetUp]
        public void Arrange()
        {
            this.are = new AutoResetEvent(false);
        }


        [Test]
        public void Create()
        {
            using(new CcrSpace())
            {}
        }

        [Test]
        public void Create_oneway_listener()
        {
            using(var s = new CcrSpace())
            {
                CcrsOneWayListener<int> l = s.CreateListener<int>(n => this.are.Set());

                l.Post(1);

                Assert.IsTrue(this.are.WaitOne(500));
            }
        }
    }
}
