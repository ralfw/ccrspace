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
    public class testCcrsOneWayListener
    {
        private AutoResetEvent are;


        [SetUp]
        public void GeneralArrangements()
        {
            this.are = new AutoResetEvent(false);
        }


        [Test]
        public void Standalone_creation()
        {
            var sut = new CcrsOneWayListener<int>(n => this.are.Set());
            sut.Post(1);
            Assert.IsTrue(this.are.WaitOne(500));
        }

    }
}
