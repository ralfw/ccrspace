using System;
using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Actors;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Actors
{
    [TestFixture]
    public class testCcrsActor : TestFixtureBase
    {
        [Test]
        public void Actor_receives_multiple_messages()
        {
            var ac = new MethodActors();
            var sut = new CcrsActor(ac.TwiceReceivingActor);
            
            sut.Post("hello");
            sut.Post("world");

            Assert.IsTrue(ac.Are.WaitOne(1000));
            Assert.AreEqual("hello", ac.ValuesReceived[0]);
            Assert.AreEqual("world", ac.ValuesReceived[1]);
        }

        
        [Test]
        public void Receives_messages_with_different_types()
        {
            var ac = new MethodActors();
            var sut = new CcrsActor(ac.MultiTypeReceivingActor);

            sut.Post("hello");
            sut.Post(true);
            sut.Post(123);

            Assert.IsTrue(ac.Are.WaitOne(1000));
            Assert.AreEqual("hello", ac.ValuesReceived[0]);
            Assert.AreEqual(123, ac.ValuesReceived[1]);
            Assert.AreEqual(true, ac.ValuesReceived[2]);
        }

        
        [Test]
        public void Actor_sends_message_to_itself()
        {
            var ac = new MethodActors();
            var sut = new CcrsActor(ac.SendToSelfActor);

            Assert.IsTrue(ac.Are.WaitOne(1000));
            Assert.AreEqual("hello", ac.ValuesReceived[0]);
        }


        [Test]
        public void Actors_in_a_dialog()
        {
            var acPong = new MethodActors();
            var sutPong = new CcrsActor(acPong.PongActor);

            var acPing = new PingActor {PongActor = sutPong};
            var sutPing = new CcrsActor(acPing);

            Assert.IsTrue(acPong.Are.WaitOne(1000));
            Assert.IsTrue(acPing.Are.WaitOne(1000));
            Assert.AreEqual(2, acPing.ValuesReceived[0]);
        }


        [Test]
        public void Actor_pulses_itself()
        {
            var ac = new MethodActors();
            var sut = new CcrsActor(ac.PulsingActor);

            Assert.IsTrue(ac.Are.WaitOne(1000));
        }
    }


    class MethodActors
    {
        public AutoResetEvent Are = new AutoResetEvent(false);
        public List<object> ValuesReceived = new List<object>();


        public IEnumerator<ITask> TwiceReceivingActor(CcrsActorContext ctx)
        {
            yield return ctx.Receive<string>();
            this.ValuesReceived.Add(ctx.ReceivedValue);

            yield return ctx.Receive<string>();
            this.ValuesReceived.Add(ctx.ReceivedValue);

            this.Are.Set();
        }


        public IEnumerator<ITask> MultiTypeReceivingActor(CcrsActorContext ctx)
        {
            yield return ctx.Receive<int, string>();
            this.ValuesReceived.Add(ctx.ReceivedValue);

            yield return ctx.Receive<int, string>();
            this.ValuesReceived.Add(ctx.ReceivedValue);

            yield return ctx.Receive<bool>();
            this.ValuesReceived.Add(ctx.ReceivedValue);

            this.Are.Set();
        }


        public IEnumerator<ITask> SendToSelfActor(CcrsActorContext ctx)
        {
            ctx.Self.PostUnknownType("hello");

            yield return ctx.Receive<string>();
            this.ValuesReceived.Add(ctx.ReceivedValue);

            this.Are.Set();
        }


        public IEnumerator<ITask> PongActor(CcrsActorContext ctx)
        {
            yield return ctx.Receive<int>();

            ctx.Reply((int) ctx.ReceivedValue + 1);

            this.Are.Set();
        }


        public IEnumerator<ITask> PulsingActor(CcrsActorContext ctx)
        {
            using (ctx.PulsePeriodically(100))
            {
                int i=0;
                while (true)
                {
                    yield return ctx.Receive<DateTime>();
                    Console.WriteLine("pulse received: {0} / {1}", ctx.ReceivedValue, i);
                    if (i++ > 3) break;
                }
            }

            this.Are.Set();
        }
    }


    class PingActor : CcrsActorBase
    {
        public Port<object> PongActor;

        public AutoResetEvent Are = new AutoResetEvent(false);
        public List<object> ValuesReceived = new List<object>();


        #region Overrides of CcrsActorBase

        public override IEnumerator<ITask> Run(CcrsActorContext ctx)
        {
            ctx.Ask(this.PongActor, 1);

            yield return ctx.Receive<int>();
            this.ValuesReceived.Add(ctx.ReceivedValue);

            this.Are.Set();
        }

        #endregion
    }
}
