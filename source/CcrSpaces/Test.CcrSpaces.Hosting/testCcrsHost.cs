using System;
using CcrSpaces.Core;
using CcrSpaces.Core.Hosting;
using GeneralTestInfrastructure;
using NUnit.Framework;
using CcrSpaces.Core.Channels;

namespace Test.CcrSpaces.Hosting
{
    [TestFixture]
    public class testCcrsHost : TestFixtureBase
    {        
        [Test]
        public void Host_oneway_channel()
        {
            using (var server = new CcrsHost("tcp.port=9999"))
            {
                server.HostPort(base.mockSpace.CreateChannel<int>(n=>base.are.Set()), "myport");

                using (var client = new CcrsHost("tcp.port=0"))
                {
                    var pc = client.ConnectToPort<int>("localhost:9999/myport");
                    pc.Post(1);

                    Assert.IsTrue(base.are.WaitOne(1000));
                }
            }
        }

        
        [Test]
        public void Connect_reqresp_channel_with_server_side_response_handling()
        {
            using (var server = new CcrsHost("tcp.port=9999"))
            {
                server.HostPort(base.mockSpace.CreateChannel<string, int>(s =>
                                                                          {
                                                                              Console.WriteLine("server: {0}", s);
                                                                              return s.Length;
                                                                          }, 
                                                                      n => base.are.Set()), 
                                                                      "myport");

                using (var client = new CcrsHost("tcp.port=0"))
                {
                    var pc = client.ConnectToPort<string>("localhost:9999/myport");
                    pc.Post("hello");

                    Assert.IsTrue(base.are.WaitOne(1000));
                }
            }
        }

        
        [Test]
        public void Connect_reqresp_channel_with_client_side_response_handling()
        {
            using (var server = new CcrsHost("tcp.port=9999"))
            {
                server.HostPort(base.mockSpace.CreateChannel<string, int>(s =>
                                                                          {
                                                                              Console.WriteLine("server: {0}", s);
                                                                              return s.Length;
                                                                          }), 
                                                                      "myport");

                using (var client = new CcrsHost("tcp.port=0"))
                {
                    var pc = client.ConnectToPort<string, int>("localhost:9999/myport");
                    pc.Request("hello").Receive(n => base.are.Set());

                    Assert.IsTrue(base.are.WaitOne(1000));
                }
            }
        }
    }
}
