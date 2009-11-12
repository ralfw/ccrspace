using System;
using CcrSpaces.Core;
using CcrSpaces.Core.Channels;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpace.Channels
{
    [TestFixture]
    public class testChannelFactoryRequestResponse : TestFixtureBase
    {
        private CcrsChannelFactory sut;

        protected override void FixtureArrange()
        {
            this.sut = new CcrsChannelFactory();
        }

        
        [Test]
        public void Create_handler_and_continuation()
        {
            int output = 0;

            var config = new CcrsRequestResponseChannelConfig<string, int>
                             {
                                 InputMessageHandler = (s, pi) => pi.Post(s.Length),
                                 OutputMessageHandler = n => {
                                                               output = n;
                                                               base.are.Set(); 
                                                             }
                             };
            var p = this.sut.CreateChannel(config);

            p.Post("hello");

            Assert.IsTrue(base.are.WaitOne(500));
            Assert.AreEqual(5, output);
        }

        
        [Test]
        public void Create_handler_without_continuation()
        {
            int output = 0;

            var config = new CcrsRequestResponseChannelConfig<string, int>
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
                        base.are.Set();
                    })
                );

            p.Post(new CcrsRequest<string, int>("hello", responses));

            Assert.IsTrue(base.are.WaitOne(500));
            Assert.AreEqual(5, output);
        }

        
        [Test]
        public void Post_without_continutation()
        {
            var config = new CcrsRequestResponseChannelConfig<string, int>
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
                                    base.are.Set();
                                })
                );
            ICausality c = new Causality("ex", pEx);
            Dispatcher.AddCausality(c);
            {
                p.Post("hello");
                Assert.IsTrue(base.are.WaitOne(500));
            }
            Dispatcher.RemoveCausality(c);
        }
    }
}
