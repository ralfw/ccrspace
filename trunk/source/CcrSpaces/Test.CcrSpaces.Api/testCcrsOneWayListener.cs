using System;
using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Api;
using CcrSpaces.Api.Config;
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


        [Test]
        public void Standalone_configuration()
        {
            var cfg = new CcrsListenerConfig<int> {MessageHandler = n=>this.are.Set()};
            var sut = new CcrsOneWayListener<int>(cfg);

            sut.Post(1);

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Standalone_fluent_configuration()
        {
            var sut = new CcrsListenerFluent<int>()
                .ProcessWith(n => this.are.Set())
                .Create();

            sut.Post(1);

            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Parallel_processing()
        {
            const int N = 300;

            List<int> numbers = new List<int>();

            var sut = new CcrsOneWayListener<int>(n =>
                              {
                                  lock (numbers) numbers.Add(n);
                                  if (n==N) this.are.Set();
                                  Thread.Sleep(20);
                              });

            for(int i=0; i<=N; i++)
                sut.Post(i);

            Assert.IsTrue(this.are.WaitOne(2000));

            bool regressionFound = false;
            int highestNumberSoFar = -1;
            for(int i=0; i<numbers.Count; i++)
            {
                if (highestNumberSoFar > numbers[i])
                {
                    regressionFound = true;
                    break;
                }
                highestNumberSoFar = numbers[i];
            }
            Assert.IsTrue(regressionFound);
        }


        [Test]
        public void Sequential_processing()
        {
            List<int> threadHashCodes = new List<int>();

            var cfg = new CcrsListenerConfig<int>
                            {
                                MessageHandler = n =>
                                {
                                    Console.WriteLine("{0} @ {1}", n, Thread.CurrentThread.GetHashCode());
                                    threadHashCodes.Add(Thread.CurrentThread.GetHashCode());
                                    Thread.Sleep(500);
                                    this.are.Set();
                                },
                                ProcessSequentially = true
                            };
            var sut = new CcrsOneWayListener<int>(cfg);

            sut.Post(1);
            sut.Post(2);

            Assert.IsTrue(this.are.WaitOne(1000));
            Assert.IsTrue(this.are.WaitOne(2000));
            Assert.AreEqual(2, threadHashCodes.Count);
            Assert.AreEqual(threadHashCodes[0], threadHashCodes[1]);
        }
    }
}
