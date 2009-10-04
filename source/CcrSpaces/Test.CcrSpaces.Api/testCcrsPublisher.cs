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
    public class testCcrsPublisher
    {
        private AutoResetEvent are1, are2;

        [SetUp]
        public void Arrange()
        {
            this.are1 = new AutoResetEvent(false);
            this.are2 = new AutoResetEvent(false);
        }


        [Test]
        public void Register_subscribers()
        {
            var sut = new MockCcrsPublisher<int>();
            Assert.AreEqual(0, sut.RegisteredSubscribers.Count);

            sut.Subscribe(Console.WriteLine);
            Assert.AreEqual(1, sut.RegisteredSubscribers.Count);

            var ch = new CcrsOneWayChannel<int>(Console.WriteLine);
            sut.Subscribe(ch);
            Assert.AreEqual(2, sut.RegisteredSubscribers.Count);
            Assert.AreSame(ch, sut.RegisteredSubscribers[1]);
        }


        [Test]
        public void Unregister_subscribers()
        {
            var sut = new MockCcrsPublisher<int>();
            var handler = new Action<int>(Console.WriteLine);
            var ch = new CcrsOneWayChannel<int>(Console.WriteLine);
            sut.Subscribe(handler);
            sut.Subscribe(ch);

            sut.Unsubscribe(handler);
            Assert.AreEqual(1, sut.RegisteredSubscribers.Count);
            Assert.AreSame(ch, sut.RegisteredSubscribers[0]);

            sut.Unsubscribe(handler);

            sut.Unsubscribe(ch);
            Assert.AreEqual(0, sut.RegisteredSubscribers.Count);

            sut.Unsubscribe(ch);
        }


        [Test]
        public void Publish()
        {
            var sut = new CcrsPublisher<int>();
            sut.Subscribe(n =>
                              {
                                  Assert.AreEqual(1, n);
                                  this.are1.Set();
                              });

            sut.Post(1);

            Assert.IsTrue(this.are1.WaitOne(500));


            sut.Subscribe(n =>
                            {
                                Assert.AreEqual(1, n);
                                this.are2.Set();
                            });

            sut.Post(1);

            Assert.IsTrue(this.are1.WaitOne(500));
            Assert.IsTrue(this.are2.WaitOne(500));
        }
    }


    internal class MockCcrsPublisher<T> : CcrsPublisher<T>
    {
        public List<ICcrsSimplexChannel<T>> RegisteredSubscribers
        {
            get
            {
                return base.subscribers;
            }
        }
    }
}
