using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Actors
{
    public abstract class CcrsActorBaseToChannel : CcrsActorBase
    {
        public CcrsActorChannel Create()
        {
            return new CcrsActorChannel(this);
        }

        public static implicit operator CcrsActorChannel(CcrsActorBaseToChannel source)
        {
            return source.Create();
        }
    }
}
