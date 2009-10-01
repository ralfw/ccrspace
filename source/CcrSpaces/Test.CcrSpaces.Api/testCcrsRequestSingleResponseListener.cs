using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Api;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testCcrsRequestSingleResponseListener
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
            var sut = new CcrsRequestSingleResponseListener<int, bool>(n => { this.are.Set(); return true; });

            sut.Post(1);

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Receive_response()
        {
            var sut = new CcrsRequestSingleResponseListener<int, bool>(n => n%2==0);

            sut.Post(1, b =>
                            {
                                Assert.IsFalse(b);
                                this.are.Set();
                            });

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Standalone_config()
        {
            var cfg = new CcrsRequestSingleResponseListenerConfig<int, bool> {MessageHandler = n=>{this.are.Set(); return true;}, TaskQueue=new DispatcherQueue()};
            var sut = new CcrsRequestSingleResponseListener<int, bool>(cfg);

            sut.Post(1);

            Assert.IsTrue(this.are.WaitOne(500));
        }
    }
}
