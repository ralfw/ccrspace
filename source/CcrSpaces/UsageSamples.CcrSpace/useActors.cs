using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Actors;
using CcrSpaces.Core;
using CcrSpaces.Hosting;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useActors
    {
        //[Test]
        //public void Let_local_and_remote_worker_chat()
        //{
        //    using (var server = new CcrSpace().ConfigureAsHost(@"tcp.port=9999"))
        //    {
        //        server.HostPort(flow, "flow");

        //        using (var client = new CcrSpace().ConfigureAsHost(@"tcp.port=0"))
        //        {
        //            var remoteFlow = client.ConnectToPort<string>(@"localhost:9999/flow");
        //            remoteFlow.Post("hello");

        //            Thread.Sleep(2000);
        //        }
        //    }
        //}
    }
}
