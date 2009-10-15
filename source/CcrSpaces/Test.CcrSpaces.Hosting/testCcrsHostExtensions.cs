using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using CcrSpaces.Channels;
using CcrSpaces.Hosting;
using GeneralTestInfrastructure;
using NUnit.Framework;

namespace Test.CcrSpaces.Hosting
{
    [TestFixture]
    public class testCcrsHostExtensions : TestFixtureBase
    {
        [Test]
        public void Configure_space_as_host()
        {
            using (var server = new CcrSpace().ConfigureAsHost("tcp.port=9999"))
            {
                server.Host(server.CreateChannel<int>(n => base.are.Set()), "myport");

                using(var client = new CcrSpace().ConfigureAsHost("tcp.port=0"))
                {
                    var p = client.ConnectToPort<int>("localhost:9999/myport");

                    p.Post(1);

                    Assert.IsTrue(base.are.WaitOne(500));
                }
            }
        }
    }
}
