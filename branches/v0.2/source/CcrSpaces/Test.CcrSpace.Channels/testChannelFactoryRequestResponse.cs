using System;
using System.Threading;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testChannelFactoryRequestResponse
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
        public void Create_handler_and_continuation()
        {
            int output = 0;

            var config = new CcrsChannelConfig<string, int>
                             {
                                 InputMessageHandler = (s, pi) => pi.Post(s.Length),
                                 OutputMessageHandler = n => {
                                                               output = n;
                                                               this.are.Set(); 
                                                             }
                             };
            var p = this.sut.CreateChannel(config);

            p.Post("hello");

            Assert.IsTrue(this.are.WaitOne(500));
            Assert.AreEqual(5, output);
        }

        
        [Test]
        public void Create_handler_without_continuation()
        {
            int output = 0;

            var config = new CcrsChannelConfig<string, int>
                            {
                                InputMessageHandler = (s, pi) => pi.Post(s.Length)
                            };
            var p = this.sut.CreateChannel(config);

            Port<int> responses = new Port<int>();
            Arbiter.Activate(
                new DispatcherQueue(),
                responses.Receive(n =>
                    {
                        output = n;
                        this.are.Set();
                    })
                );

            p.Post(new CcrsRequest<string, int>("hello", responses));

            Assert.IsTrue(this.are.WaitOne(500));
            Assert.AreEqual(5, output);
        }

        
        [Test]
        public void Post_without_continutation()
        {
            var config = new CcrsChannelConfig<string, int>
                            {
                                InputMessageHandler = (s, pi) => pi.Post(s.Length)
                            };
            var p = this.sut.CreateChannel(config);


            Port<Exception> pEx = new Port<Exception>();
            Arbiter.Activate(
                new DispatcherQueue(),
                pEx.Receive(ex =>
                                {
                                    Console.WriteLine(ex.Message);
                                    this.are.Set();
                                })
                );
            ICausality c = new Causality("ex", pEx);
            Dispatcher.AddCausality(c);
            {
                p.Post("hello");
                Assert.IsTrue(this.are.WaitOne(500));
            }
            Dispatcher.RemoveCausality(c);
        }
    }
}
