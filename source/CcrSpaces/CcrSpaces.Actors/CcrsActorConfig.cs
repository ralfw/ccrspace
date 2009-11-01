using System;
using System.Collections.Generic;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Actors
{
    public class CcrsActorConfig
    {
        public Func<CcrsActorContext, IEnumerator<ITask>> MessageHandler;

        public DispatcherQueue TaskQueue;
    }
}
