using System.Threading;
using CcrSpaces.Channels;
using CcrSpaces.PubSub;
using GeneralTestInfrastructure;
using NUnit.Framework;

namespace Test.CcrSpaces.PubSub
{
    [TestFixture]
    public class testPublicationHub : TestFixtureBase
    {
        [Test]
        public void Subscribe_and_Publish()
        {
            var sut = new CcrsPublicationHub<string>(new CcrsPublicationHubConfig());

            var chf = new CcrsChannelFactory();
            var sub1 = chf.CreateChannel(new CcrsChannelConfig<string>
                                                   {
                                                       MessageHandler = s => base.are.Set()
                                                   });

            var are2 = new AutoResetEvent(false);
            var sub2 = chf.CreateChannel(new CcrsChannelConfig<string>
                                                    {
                                                        MessageHandler = s => are2.Set()
                                                    });

            sut.Subscriptions.Post(new CcrsSubscribe<string>("s1", sub1));
            sut.Subscriptions.Post(new CcrsSubscribe<string>("s2", sub2));

            Thread.Sleep(1000); // wait for subscription to be processed

            sut.Post("hello");

            Assert.IsTrue(base.are.WaitOne(500));
            Assert.IsTrue(are2.WaitOne(500));
        }

        
        [Test]
        public void Remove_subscription()
        {
            var sut = new CcrsPublicationHub<string>(new CcrsPublicationHubConfig());

            var chf = new CcrsChannelFactory();
            var sub1 = chf.CreateChannel(new CcrsChannelConfig<string>
                            {
                                MessageHandler = s => base.are.Set()
                            });


            sut.Subscriptions.Post(new CcrsSubscribe<string>("s1", sub1));
            Thread.Sleep(1000); // wait for subscription to be processed

            sut.Post("hello");

            Assert.IsTrue(base.are.WaitOne(500));

            sut.Subscriptions.Post(new CcrsUnsubscribe("s1"));
            Thread.Sleep(1000); // wait for unsubscribe to be processed

            sut.Post("world");

            Assert.IsFalse(base.are.WaitOne(500));
        }
    }
}
