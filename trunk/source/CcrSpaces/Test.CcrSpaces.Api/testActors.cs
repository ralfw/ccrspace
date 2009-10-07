using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Api;
using CcrSpaces.Api.Actors;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class testActors
    {
        private AutoResetEvent are;

        [SetUp]
        public void Arrange()
        {
            this.are = new AutoResetEvent(false);
        }

        
        [Test]
        public void Create_from_class()
        {
            using (var space = new CcrSpace())
            {
                var actor = new ReceivingActor();

                var a = space.CreateActor(actor);
                a.Post("hello");
                a.Post(123);

                Assert.IsTrue(actor.Wait(500));
                Assert.AreEqual(2, actor.ReceivedValues.Count);
                Assert.AreEqual("hello", actor.ReceivedValues[0]);
                Assert.AreEqual(123, actor.ReceivedValues[1]);
            }
        }


        [Test]
        public void Create_from_method_and_receive_with_multiple_types()
        {
            using (var space = new CcrSpace())
            {
                var actor = new ReceivingActor();

                var a = space.CreateActor(actor.ReceiveWithMultipleTypes);
                a.Post(true);
                a.Post(123);
                a.Post("stop");

                Assert.IsTrue(actor.Wait(500));
                Assert.AreEqual(3, actor.ReceivedValues.Count);
                Assert.AreEqual(true, actor.ReceivedValues[0]);
                Assert.AreEqual(123, actor.ReceivedValues[1]);
                Assert.AreEqual("stop", actor.ReceivedValues[2]);
            }
        }


        [Test]
        public void Send_to_self()
        {
            using (var space = new CcrSpace())
            {
                var actor = new ReceivingActor();

                var a = space.CreateActor(actor.SendToSelf);

                Assert.IsTrue(actor.Wait(500));
                Assert.AreEqual(1, actor.ReceivedValues.Count);
                Assert.AreEqual("hello", actor.ReceivedValues[0]);
            }
        }


        [Test]
        public void Actor_conversation_with_reply()
        {
            var replying = new ReplyingActor();
            var chReplying = new CcrsActorChannel(replying);

            var sending = new SendingActor {chReplying = chReplying};
            var chSending = new CcrsActorChannel(sending);

            Assert.IsTrue(sending.Wait(500));
            Assert.AreEqual(2, sending.ReceivedValues.Count);
            Assert.AreEqual("quick", sending.ReceivedValues[0]);
            Assert.AreEqual("fox", sending.ReceivedValues[1]);
            Assert.AreEqual(2, replying.ReceivedValues.Count);
            Assert.AreEqual("the", replying.ReceivedValues[0]);
            Assert.AreEqual("brown", replying.ReceivedValues[1]);
        }


        private class ReceivingActor : TestActorBase
        {
            public readonly List<object> ReceivedValues = new List<object>();

            internal override IEnumerator<ITask> Run(CcrsActorContext ctx)
            {
                yield return ctx.Receive<string>();
                this.ReceivedValues.Add(ctx.ReceivedValue);

                yield return ctx.Receive<int>();
                this.ReceivedValues.Add(ctx.ReceivedValue);

                this.are.Set();
            }


            public IEnumerator<ITask> ReceiveWithMultipleTypes(CcrsActorContext ctx)
            {
                while (true)
                {
                    yield return ctx.Receive<string, int, bool>();
                    this.ReceivedValues.Add(ctx.ReceivedValue);

                    if (ctx.ReceivedValue is string && 
                       (string)ctx.ReceivedValue == "stop")
                        this.are.Set();
                }
            }


            public IEnumerator<ITask> SendToSelf(CcrsActorContext ctx)
            {
                ctx.Self.Post("hello");
                yield return ctx.Receive<string>();
                this.ReceivedValues.Add(ctx.ReceivedValue);
                this.are.Set();
            }
        }



        private class SendingActor : TestActorBase
        {
            public readonly List<object> ReceivedValues = new List<object>();
            public CcrsActorChannel chReplying;


            #region Overrides of CcrsActorBase

            internal override IEnumerator<ITask> Run(CcrsActorContext ctx)
            {
                this.chReplying.Post("the", ctx.Self);
                yield return ctx.Receive<string>();
                this.ReceivedValues.Add(ctx.ReceivedValue);
                Console.WriteLine("s {0}", ctx.ReceivedValue);

                ctx.Reply("brown");
                yield return ctx.Receive<string>();
                this.ReceivedValues.Add(ctx.ReceivedValue);
                Console.WriteLine("s {0}", ctx.ReceivedValue);

                this.are.Set();
            }

            #endregion
        }


        private class ReplyingActor : TestActorBase
        {
            public readonly List<object> ReceivedValues = new List<object>();

            #region Overrides of CcrsActorBase

            internal override IEnumerator<ITask> Run(CcrsActorContext ctx)
            {
                yield return ctx.Receive<string>();
                this.ReceivedValues.Add(ctx.ReceivedValue);
                Console.WriteLine("r {0}", ctx.ReceivedValue);

                ctx.Reply("quick");

                yield return ctx.Receive<string>();
                this.ReceivedValues.Add(ctx.ReceivedValue);
                Console.WriteLine("r {0}", ctx.ReceivedValue);

                ctx.Reply("fox");
            }

            #endregion
        }


        private abstract class TestActorBase : CcrsActorBase
        {
            protected readonly AutoResetEvent are = new AutoResetEvent(false);

            public bool Wait(int msec)
            {
                return this.are.WaitOne(msec);
            }
        }
    }
}
