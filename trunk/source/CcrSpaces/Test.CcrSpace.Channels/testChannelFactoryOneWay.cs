using System;
using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Channels;
using GeneralTestInfrastructure;
using NUnit.Framework;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testChannelFactoryOneWay : TestFixtureBase
    {
        private CcrsChannelFactory sut;

        protected override void FixtureArrange()
        {
            this.sut = new CcrsChannelFactory();
        }


        [Test]
        public void Create_persistent_port_with_handler()
        {
            var cfg = new CcrsOneWayChannelConfig<int> {MessageHandler = n => base.are.Set()};
            var p = this.sut.CreateChannel(cfg);

            p.Post(1);
            p.Post(2);

            Assert.IsTrue(base.are.WaitOne(1000));
            Assert.IsTrue(base.are.WaitOne(1000));
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

            var cfg = new CcrsOneWayChannelConfig<int>
                          {
                              MessageHandler = s =>
                                                    {
                                                        for (int i = 0; i < 100; i++)
                                                        {
                                                            lock (numbers)
                                                                numbers.Add(s + i);
                                                            Thread.Sleep(20);
                                                        }
                                                        if (s == 1) base.are.Set();
                                                    },
                                                    HandlerMode = mode
                          };
            var p = this.sut.CreateChannel(cfg);

            p.Post(1);
            p.Post(10000);

            Assert.IsTrue(base.are.WaitOne(4000));

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
