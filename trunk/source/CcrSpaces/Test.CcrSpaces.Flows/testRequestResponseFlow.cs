using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using CcrSpaces.ExceptionHandling;
using CcrSpaces.Flows;
using CcrSpaces.Flows.Infrastructure;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using Microsoft.Ccr.Core.Arbiters;
using NUnit.Framework;
using System.Runtime.Remoting;

namespace Test.CcrSpaces.Flows
{
    [TestFixture]
    public class testRequestResponseFlow : TestFixtureBase
    {
        #region Arrange
        private PortSet<string, CcrsRequest<string, int>, CcrsRequestOfUnknownType> chString2Int;

        protected override void FixtureArrange()
        {
            this.chString2Int = new CcrsChannelFactory().CreateChannel(new CcrsRequestResponseChannelConfig<string, int>
                                     {
                                         InputMessageHandler = (s, p) => 
                                         {
                                             Console.WriteLine("string2int: {0}", s);
                                             p.Post(s.Length); 
                                         }
                                     });
        }
        #endregion


        [Test]
        public void Single_stage_reqresp_flow()
        {
            var f = new CcrsFlow<string, int>(new Port<CcrsRequestOfUnknownType>[] {this.chString2Int});

            f.Request("hello").Receive(n => base.are.Set());

            Assert.IsTrue(base.are.WaitOne(500));
        }


        #region spike
        //[Test]
        //public void Create_one_way_flow()
        //{
        //    var f = new CcrsFlow()
        //        .Start<string, int>(s => s.Length)
        //        .Continue(n => 2*n)
        //        .Continue(n => n % 2 == 0)
        //        .Finish(b => { Console.WriteLine(b); base.are.Set(); });

        //    f.Post("hello");

        //    Assert.IsTrue(base.are.WaitOne(500));
        //}

        [Test]
        public void xxx()
        {
            var chf = new CcrsChannelFactory();

            var chStringLength = new CcrsChannelFactory().CreateChannel<string, int>(new CcrsRequestResponseChannelConfig<string, int>
                                     {
                                         InputMessageHandler = (s, p) => 
                                         {
                                             Console.WriteLine("s: {0}", s);
                                             p.Post(s.Length); 
                                         }
                                     });

            var chIsEvenNumber = new CcrsChannelFactory().CreateChannel<int, bool>(new CcrsRequestResponseChannelConfig<int, bool>
                                    {
                                        InputMessageHandler = (n, p) => 
                                        {
                                            Console.WriteLine("n: {0}", n);
                                            p.Post(n%2 ==0); 
                                        }
                                    });

            var chDumpBool = new CcrsChannelFactory().CreateChannel<bool>(new CcrsOneWayChannelConfig<bool>
                                    {
                                        MessageHandler = b =>
                                         {
                                             Console.WriteLine("result: {0}", b);
                                             base.are.Set();
                                         }
                                    });


            using (new CcrsCausality(Console.WriteLine))
            {
                IPortReceive pLengthToEven = (IPortReceive)Activator.CreateInstance(chIsEvenNumber.P0.GetType());
                pLengthToEven.RegisterReceiver(new IPortReceiveExtensions.ConsumingReceiverTask(e => chIsEvenNumber.P2.Post(new CcrsRequestOfUnknownType(e, chDumpBool))));

                IPortReceive pInput = (IPortReceive)Activator.CreateInstance(chStringLength.P0.GetType());
                pInput.RegisterReceiver(new IPortReceiveExtensions.ConsumingReceiverTask(e => chStringLength.P2.Post(new CcrsRequestOfUnknownType(e, (IPort)pLengthToEven))));

                (pInput as Port<string>).Post("hello");
                (pInput as Port<string>).Post("world");

                Assert.IsTrue(base.are.WaitOne(500));
                Assert.IsTrue(base.are.WaitOne(500));
            }
        }
        #endregion
    }
}
