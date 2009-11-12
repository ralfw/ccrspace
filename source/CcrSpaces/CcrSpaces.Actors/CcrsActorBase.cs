using System.Collections.Generic;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Actors
{
    public abstract class CcrsActorBase
    {
        public abstract IEnumerator<ITask> Run(CcrsActorContext ctx);
    }
}
