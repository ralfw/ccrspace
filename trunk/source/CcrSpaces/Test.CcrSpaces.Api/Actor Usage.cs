using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api;
using Microsoft.Ccr.Core;
using NUnit.Framework;

namespace Test.CcrSpaces.Api
{
    [TestFixture]
    public class Actor_Usage
    {
        [Test]
        public void Create_actor_from_method()
        {
            using (var space = new CcrSpace())
            {
                var actor = space.CreateActor(ActorMethod);
                actor.Post("hello");
            }
        }


        IEnumerator<ITask> ActorMethod(CcrsActorContext ctx)
        {
            yield return ctx.Receive<string>();
            Console.WriteLine(ctx.ReceivedValue);

            yield return ctx.Receive<int, bool>();
            Console.WriteLine(ctx.ReceivedValue);
        }
    }
}
