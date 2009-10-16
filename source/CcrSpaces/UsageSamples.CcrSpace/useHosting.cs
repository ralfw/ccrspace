using System;
using System.Threading;
using NUnit.Framework;

using CcrSpaces.Core;
using CcrSpaces.Channels;
using CcrSpaces.Hosting;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useHosting
    {
        [Test]
        public void Echo_server()
        {
            using(var server = new CcrSpace().ConfigureAsHost("tcp.port=9999"))
            {
                var echoService = server.CreateChannel<string, string>(EchoProcessor);
                server.HostPort(echoService, "echoservice");

                using(var client = new CcrSpace().ConfigureAsHost("tcp.port=0"))
                {
                    var remoteEchoService = client.ConnectToPort<string, string>("localhost:9999/echoservice");
                    remoteEchoService
                        .Request("hello, world!")
                        .Receive(s => Console.WriteLine("echo received: '{0}'", s));

                    Thread.Sleep(2000);
                }
            }
        }


        private static string EchoProcessor(string text)
        {
            Console.WriteLine("  request to echo back '{0}'", text);
            return text.ToUpper();
        }
    }
}
