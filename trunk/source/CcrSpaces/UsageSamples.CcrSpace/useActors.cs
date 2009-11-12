using System;
using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Core;
using CcrSpaces.Core.Actors;
using CcrSpaces.Core.Core;
using CcrSpaces.Core.Hosting;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace UsageSamples.CcrSpaces
{
    [TestFixture]
    public class useActors
    {
        [Test]
        public void Let_local_and_remote_worker_chat()
        {
            using (var server = new CcrSpace().ConfigureAsHost(@"tcp.port=9999"))
            {
                server.HostPort(server.CreateActor(new PongActor()), "pong");

                using (var client = new CcrSpace().ConfigureAsHost(@"tcp.port=0"))
                {
                    var remoteActor = client.ConnectToPort<object>(@"localhost:9999/pong");

                    var localActor = client.CreateActor(new PingActor(remoteActor));
                    localActor.Post("hello, pong!");

                    Thread.Sleep(2000);
                }
            }
        }
    }


    class PingActor : CcrsActorBase
    {
        private readonly Port<object> pongActor;

        public PingActor(Port<object> pongActor)
        { this.pongActor = pongActor; }


        #region Overrides of CcrsActorBase
        public override IEnumerator<ITask> Run(CcrsActorContext ctx)
        {
            Console.WriteLine("waiting for message to send to pong");
            yield return ctx.Receive<string>();

            Console.WriteLine("pinging pong actor with '{0}'", ctx.ReceivedValue);
            ctx.Ask(this.pongActor, ctx.ReceivedValue);

            Console.WriteLine("waiting for response");
            yield return ctx.Receive<string>();

            Console.WriteLine("received '{0}' from pong", ctx.ReceivedValue);
        }
        #endregion
    }


    class PongActor : CcrsActorBase
    {
        #region Overrides of CcrsActorBase

        public override IEnumerator<ITask> Run(CcrsActorContext ctx)
        {
            Console.WriteLine("    waiting for message from ping");

            yield return ctx.Receive<string>();
            Console.WriteLine("    received '{0}' from ping", ctx.ReceivedValue);

            ctx.Reply(string.Format("hi, there! here´s your message back: {0}", ctx.ReceivedValue.ToString().ToUpper()));
            Console.WriteLine("    replied to ping");
        }

        #endregion
    }
}
