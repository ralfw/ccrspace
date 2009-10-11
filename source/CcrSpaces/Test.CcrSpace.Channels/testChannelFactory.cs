using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Core;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testChannelFactory
    {
        private AutoResetEvent are;
        private ChannelFactory sut;

        [SetUp]
        public void GlobalArrange()
        {
            this.are = new AutoResetEvent(false);
            this.sut = new ChannelFactory();
        }


        [Test]
        public void Create_persistent_port_with_handler()
        {
            var cfg = new CcrsChannelConfig<int> {MessageHandler = n => this.are.Set()};
            var p = this.sut.CreateChannel<int>(cfg);

            p.Post(1);
            p.Post(2);

            Assert.IsTrue(this.are.WaitOne(500));
            Assert.IsTrue(this.are.WaitOne(1000));
        }


        [Test]
        public void Handle_messages_sequentially()
        {
            Process_messages(CcrsChannelHandlerModes.Sequential, Assert.AreEqual);
        }

        [Test]
        public void Parallel_message_processing()
        {
            Process_messages(CcrsChannelHandlerModes.Parallel, Assert.Less);
        }


        private void Process_messages(CcrsChannelHandlerModes mode, Action<int, int> assertListWasFilledCorrectly)
        {
            List<int> numbers = new List<int>();

            var cfg = new CcrsChannelConfig<int>
                          {
                              MessageHandler = s =>
                                                    {
                                                        for (int i = 0; i < 100; i++)
                                                        {
                                                            lock (numbers)
                                                                numbers.Add(s + i);
                                                            Thread.Sleep(20);
                                                        }
                                                        if (s == 1) this.are.Set();
                                                    },
                                                    HandlerMode = mode
                          };
            var p = this.sut.CreateChannel<int>(cfg);

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
    }
}
