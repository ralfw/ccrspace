using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Actors
{
    [Serializable]
    internal class ActorDialogMessage
    {
        public object Message;
        public IPort Sender;
    }
}
