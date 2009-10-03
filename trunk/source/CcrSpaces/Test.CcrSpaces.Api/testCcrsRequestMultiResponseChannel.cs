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
    public class testCcrsRequestMultiResponseChannel
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
            var sut = new CcrsRequestMultiResponseChannel<int, bool>((n, p) => this.are.Set());

            sut.Post(1);

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Receive_single_response()
        {
            var sut = new CcrsRequestMultiResponseChannel<int, bool>((n,p) => p.Post(n%2==0));

            sut.Post(1, b =>
                            {
                                Assert.IsFalse(b);
                                this.are.Set();
                            });

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Receive_multiple_responses()
        {
            var cfg = new CcrsRequestMultiResponseChannelConfig<int, int>
                          {
                              MessageHandler = (n, p) =>
                              {
                                  p.Post(n + 1);
                                  p.Post(n + 2);
                              },
                              TaskQueue = new DispatcherQueue(),
                              ProcessSequentially = true
                          };
            var sut = new CcrsRequestMultiResponseChannel<int, int>(cfg);

            List<int> numbers = new List<int>();
            sut.Post(1, n =>
                        {
                            numbers.Add(n);
                            if (n==3) this.are.Set();
                        });

            Assert.IsTrue(this.are.WaitOne(500));
            Assert.AreEqual(2, numbers[0]);
            Assert.AreEqual(3, numbers[1]);
        }


        [Test]
        public void Standalone_config()
        {
            var cfg = new CcrsRequestMultiResponseChannelConfig<int, bool> {MessageHandler = (n, p)=>this.are.Set(), TaskQueue=new DispatcherQueue()};
            var sut = new CcrsRequestMultiResponseChannel<int, bool>(cfg);

            sut.Post(1);

            Assert.IsTrue(this.are.WaitOne(500));
        }
    }
}
