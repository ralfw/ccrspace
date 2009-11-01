using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CcrSpaces.Actors;
using CcrSpaces.Core;
using GeneralTestInfrastructure;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Actors
{
    [TestFixture]
    public class testCcrSpaceExtensions : TestFixtureBase
    {
        [Test]
        public void Sends_message_to_actor()
        {
            using(var space = new CcrSpace())
            {
                var ac = new SimpleActor();
                var sut = space.CreateActor(ac.ActorMethod);

                sut.Post("hello");

                Assert.IsTrue(ac.Are.WaitOne(500));
            }
        }
    }

    class SimpleActor
    {
        public AutoResetEvent Are = new AutoResetEvent(false);

        public IEnumerator<ITask> ActorMethod(CcrsActorContext ctx)
        {
            yield return ctx.Receive<string>();

            if (ctx.ReceivedValue.Equals("hello")) this.Are.Set();
        }
    }
}
