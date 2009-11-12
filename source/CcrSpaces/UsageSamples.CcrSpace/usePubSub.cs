using System;
using System.Threading;
using CcrSpaces.Core;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Hosting;
using CcrSpaces.Core.PubSub;
using NUnit.Framework;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class usePubSub
    {
        [Test]
        public void Remote_PubSub()
        {
            using (var server = new CcrSpace().ConfigureAsHost(@"tcp.port=9999"))
            {
                var pub = server.CreatePublicationHub<string>();
                server.HostAsyncComponent(pub.Subscriptions, "pubsub");

                using (var client = new CcrSpace().ConfigureAsHost(@"tcp.port=0"))
                {
                    var pubsub = client.ConnectToAsyncComponent<CcrsSubscriptionManager<string>>(@"localhost:9999/pubsub");

                    pubsub.Post(new CcrsSubscribe<string>("sub", client.CreateChannel<string>(s => Console.WriteLine("Received: {0}", s))));
                    Thread.Sleep(1000); // give time to process subscription

                    Console.WriteLine("publishing!");
                    pub.Post("hello, world!");

                    Thread.Sleep(2000);
                }
            }
        }
    }
}
