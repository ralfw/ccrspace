using CcrSpaces.Core;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Hosting;
using GeneralTestInfrastructure;
using NUnit.Framework;

namespace Test.CcrSpaces.Hosting
{
    [TestFixture]
    public class testCcrSpaceExtensions : TestFixtureBase
    {
        [Test]
        public void Configure_space_as_host()
        {
            using (var server = new CcrSpace().ConfigureAsHost("tcp.port=9999"))
            {
                server.HostPort(server.CreateChannel<int>(n => base.are.Set()), "myport");

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
