using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Actors
{
    public class CcrsActorConfig
    {
        public Func<CcrsActorContext, IEnumerator<ITask>> MessageHandler;

        public DispatcherQueue TaskQueue;
    }
}
