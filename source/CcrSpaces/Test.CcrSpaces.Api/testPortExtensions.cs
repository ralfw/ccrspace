using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Ccr.Core;
using NUnit.Framework;
using CcrSpaces.Infrastructure;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testPortExtensions
    {
        private AutoResetEvent are;

        [SetUp]
        public void Arrange()
        {
            this.are = new AutoResetEvent(false);
        }


        [Test]
        public void Port_handles_multiple_messages()
        {
            var p = new Port<int>();
            p.RegisterHandler(n => this.are.Set(), new DispatcherQueue(), false, false);

            p.Post(1);
            Assert.IsTrue(this.are.WaitOne(500));

            p.Post(2);
            Assert.IsTrue(this.are.WaitOne(500));
        }


        [Test]
        public void Parallel_message_processing()
        {
            Process_messages(false, Assert.Less);
        }


        [Test]
        public void Sequential_message_processing()
        {
            Process_messages(true, Assert.AreEqual);
        }


        private void Process_messages(bool processSequentially, Action<int, int> assertListWasFilledCorrectly)
        {
            List<int> numbers = new List<int>();

            var p = new Port<int>();
            p.RegisterHandler(s =>
                                {
                                    for (int i = 0; i < 100; i++)
                                    {
                                        lock (numbers)
                                            numbers.Add(s + i);
                                        Thread.Sleep(20);
                                    }
                                    if (s == 1) this.are.Set();
                                }, 
                                new DispatcherQueue(), 
                                processSequentially,
                                false);

            p.Post(1);
            p.Post(10000);

            Assert.IsTrue(this.are.WaitOne(4000));

            int j = 1;
            while (j < numbers.Count)
            {
                if (numbers[j - 1] > numbers[j]) break;
                j++;
            }

            assertListWasFilledCorrectly(j, numbers.Count);
        }


        [Test]
        public void Processing_with_iterator()
        {
            Arbiter.Activate(
                new DispatcherQueue(),
                new IterativeTask<int>(10, Count)
                );

            Assert.IsTrue(this.are.WaitOne(500));
        }

        IEnumerator<ITask> Count(int start)
        {
            int n = 0;
            var p = new Port<object>();

            p.Post("hello");
            p.Post(start);
            yield return new Receiver<object>(false, p, o=>o is int, new Task<object>(o => n=(int)o));

            yield return new Receiver<object>(false, p, o => o is string, new Task<object>(Console.WriteLine));

            Console.WriteLine("received: {0}", n);
            this.are.Set();
        }
    }
}
