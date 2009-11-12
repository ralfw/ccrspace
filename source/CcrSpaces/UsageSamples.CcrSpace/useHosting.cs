using System;
using System.Threading;
using CcrSpaces.Core;
using NUnit.Framework;

using CcrSpaces.Core.Core;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Hosting;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useHosting
    {
        [Test]
        public void Echo_server()
        {
            /* Hosting config strings for different transport media:
             * 
             *   TCP/Sockets
             *      server: tcp.port=9999
             *      client: tcp.port=0
             *          remote address: localhost:9999/echoservice
             *   WCF
             *      server: wcf.port=9999
             *      client: wcf.port=0
             *          remote address: localhost:9999/echoservice
             *      
             *   MSMQ (needs to be installed explicitly on your computer!)
             *      server: msmq.queuename=.\private$\serverqueue
             *      client: msmq.queuename=.\private$\clientqueue
             *          remote address: .\private$\serverqueue/echoservice
             *          
             *   Jabber
             *      server: jabber.jid=<your jabber id>/server;jabber.password=<your jabber pwd>
             *      client: jabber.jid=<your jabber id>/client;jabber.password=<your jabber pwd>
             *          remote address: <your jabber id>/server/echoservice
             */
            using (var server = new CcrSpace().ConfigureAsHost(@"tcp.port=9999"))
            {
                var echoService = server.CreateChannel<string, string>(EchoProcessor);
                server.HostPort(echoService, "echoservice");

                using(var client = new CcrSpace().ConfigureAsHost(@"tcp.port=0"))
                {
                    var remoteEchoService = client.ConnectToPort<string, string>(@"localhost:9999/echoservice");
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
