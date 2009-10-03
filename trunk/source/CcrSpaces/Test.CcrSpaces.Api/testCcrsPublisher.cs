using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testCcrsPublisher
    {
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
