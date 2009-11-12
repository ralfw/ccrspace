using System;
using System.Collections.Generic;
using CcrSpaces.Core.Actors;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public class CcrsActorConfig
    {
        public Func<CcrsActorContext, IEnumerator<ITask>> MessageHandler;

        public DispatcherQueue TaskQueue;
    }
}
