using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Actors
{
    public abstract class CcrsActorBase
    {
        internal abstract IEnumerator<ITask> Run(CcrsActorContext ctx);
    }
}
