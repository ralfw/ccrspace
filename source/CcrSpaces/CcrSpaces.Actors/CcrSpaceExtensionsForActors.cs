using System;
using System.Collections.Generic;
using CcrSpaces.Core.Actors;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public static class CcrSpaceExtensionsForActors
    {
        public static CcrsActor CreateActor(this ICcrSpace space, CcrsActorBase actor)
        { return CreateActor(space, actor.Run); }
        public static CcrsActor CreateActor(this ICcrSpace space, Func<CcrsActorContext, IEnumerator<ITask>> actorMethod)
        { return CreateActor(space, new CcrsActorConfig {MessageHandler = actorMethod}); }
        public static CcrsActor CreateActor(this ICcrSpace space, CcrsActorConfig config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return new CcrsActor(config);
        }
    }
}
