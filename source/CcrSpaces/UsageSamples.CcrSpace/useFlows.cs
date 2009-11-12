using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Core;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Core;
using CcrSpaces.Core.Hosting;
using CcrSpaces.Core.Flows;
using NUnit.Framework;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useFlows
    {
        [Test]
        public void Start_remote_workflow()
        {
            using (var server = new CcrSpace().ConfigureAsHost(@"tcp.port=9999"))
            {
                var flow = server.StartFlow<string, string>(s => s.ToUpper())
                            .Continue(s => s.ToCharArray())
                            .Continue(s => new string(s.Reverse().ToArray()))
                            .Terminate(s => Console.WriteLine("server side transformation result: {0}", s));
                
                server.HostPort(flow, "flow");

                using (var client = new CcrSpace().ConfigureAsHost(@"tcp.port=0"))
                {
                    var remoteFlow = client.ConnectToPort<string>(@"localhost:9999/flow");
                    remoteFlow.Post("hello");

                    Thread.Sleep(2000);
                }
            }
        }

        
        [Test]
        public void Call_remote_flow_and_receive_result()
        {
            using (var server = new CcrSpace().ConfigureAsHost(@"tcp.port=9999"))
            {
                var flow = server.StartFlow<string, string>(s => s.ToUpper())
                            .Continue(s => s.ToCharArray())
                            .Continue(s => new string(s.Reverse().ToArray()));

                server.HostPort(flow, "flow");

                using (var client = new CcrSpace().ConfigureAsHost(@"tcp.port=0"))
                {
                    var remoteFlow = client.ConnectToPort<string, string>(@"localhost:9999/flow");
                    remoteFlow
                        .Request("hello")
                        .Receive(s => Console.WriteLine("received: '{0}'", s));

                    Thread.Sleep(2000);
                }
            }
        }
    }
}
