using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using CcrSpaces.ExceptionHandling;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using Microsoft.Ccr.Core.Arbiters;
using NUnit.Framework;
using System.Runtime.Remoting;

namespace Test.CcrSpaces.Flows
{
    class CcrsFlow
    {
        public CcrsFlow<TInput, TOutput> Start<TInput, TOutput>(Func<TInput, TOutput> dataHandler)
        {
            return new CcrsFlow<TInput, TOutput>(new[]{new CcrsChannelFactory()
                                                    .CreateChannel(
                                                        new CcrsChannelConfig<TInput, TOutput>
                                                            {
                                                                InputMessageHandler = (e, p) => p.Post(dataHandler(e)),
                                                                InputHandlerMode = CcrsChannelHandlerModes.Sequential
                                                            }).P2});
        }
    }


    class CcrsFlow<TInput, TOutput> : PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType>
    {
        private readonly List<Port<CcrsRequestOfUnknownType>> stages;


        internal CcrsFlow(IEnumerable<Port<CcrsRequestOfUnknownType>> stages)
        {
            this.stages = new List<Port<CcrsRequestOfUnknownType>>(stages);
        }


        public CcrsFlow<TInput, TNewOutput> Continue<TNewOutput>(Func<TOutput, TNewOutput> dataHandler)
        {
            return new CcrsFlow<TInput, TNewOutput>(
                this.stages.Concat(new[] {new CcrsChannelFactory()
                                                .CreateChannel(new CcrsChannelConfig<TOutput, TNewOutput>
                                                                {
                                                                    InputMessageHandler = (e, p) => p.Post(dataHandler(e)),
                                                                    InputHandlerMode = CcrsChannelHandlerModes.Sequential
                                                                }).P2}));
        }


        public CcrsFlow<TInput> Finish(Action<TOutput> dataHandler)
        {
            return null;
        }
    }


    class CcrsFlow<TInput> : Port<TInput>
    {}


    [TestFixture]
    public class Class1 : TestFixtureBase
    {
        [Test]
        public void Create_one_way_flow()
        {
            var f = new CcrsFlow()
                .Start<string, int>(s => s.Length)
                .Continue(n => 2*n)
                .Continue(n => n % 2 == 0)
                .Finish(b => { Console.WriteLine(b); base.are.Set(); });

            f.Post("hello");

            Assert.IsTrue(base.are.WaitOne(500));
        }

        [Test]
        public void xxx()
        {
            var chf = new CcrsChannelFactory();

            var chStringLength = new CcrsChannelFactory().CreateChannel<string, int>(new CcrsChannelConfig<string, int>
                                     {
                                         InputMessageHandler = (s, p) => 
                                         {
                                             Console.WriteLine("s: {0}", s);
                                             p.Post(s.Length); 
                                         }
                                     });

            var chIsEvenNumber = new CcrsChannelFactory().CreateChannel<int, bool>(new CcrsChannelConfig<int, bool>
                                    {
                                        InputMessageHandler = (n, p) => 
                                        {
                                            Console.WriteLine("n: {0}", n);
                                            p.Post(n%2 ==0); 
                                        }
                                    });

            var chDumpBool = new CcrsChannelFactory().CreateChannel<bool>(new CcrsChannelConfig<bool>
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
                pLengthToEven.RegisterReceiver(new SyncReceiverTask(e => chIsEvenNumber.P2.Post(new CcrsRequestOfUnknownType(e, chDumpBool))));

                IPortReceive pInput = (IPortReceive)Activator.CreateInstance(chStringLength.P0.GetType());
                pInput.RegisterReceiver(new SyncReceiverTask(e => chStringLength.P2.Post(new CcrsRequestOfUnknownType(e, (IPort)pLengthToEven))));

                (pInput as Port<string>).Post("hello");

                Assert.IsTrue(base.are.WaitOne(500));
            }
        }


        [Test]
        public void Test_port_pass_on()
        {
            var p = new Port<int>();

            (p as IPortReceive).RegisterReceiver(new SyncReceiverTask(Console.WriteLine));

            p.Post(1);
            p.Post(2);

            int n;
            Assert.IsFalse(p.Test(out n));
        }
    }




    class SyncReceiverTask : ReceiverTask
    {
        private readonly Action<object> elementHandler;

        public SyncReceiverTask(Action<object> elementHandler)
        {
            base.State = ReceiverTaskState.Persistent;
            this.elementHandler = elementHandler;
        }

        public override void Cleanup(ITask task)
        {
            // nothing to do
        }

        public override bool Evaluate(IPortElement messageNode, ref ITask deferredTask)
        {
            deferredTask = null; // don´t try to queue the element
            this.elementHandler(messageNode.Item);
            return true;
        }

        public override void Consume(IPortElement item)
        {}
    }
}
