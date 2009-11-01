using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Actors;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Actors
{
    public abstract class CcrsActorBase
    {
        public abstract IEnumerator<ITask> Run(CcrsActorContext ctx);
    }
}
