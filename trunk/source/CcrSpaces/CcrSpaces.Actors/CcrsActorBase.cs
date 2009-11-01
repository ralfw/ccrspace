using System.Collections.Generic;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Actors
{
    public abstract class CcrsActorBase
    {
        public abstract IEnumerator<ITask> Run(CcrsActorContext ctx);
    }
}
