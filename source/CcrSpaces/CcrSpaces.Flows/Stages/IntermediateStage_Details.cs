using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows.Stages
{
    partial class IntermediateStage<TInput, TOutput>
    {
        private void AssertHasResponsePort(StageMessage m)
        {
            if (base.Next == null && m.ResponsePort == null) throw new InvalidOperationException("Stage cannot process message due to missing next stage and no response port being defined!");
        }

        private bool IsTerminalStage()
        {
            return base.Next == null;
        }

        private void SendResultBackToCaller(StageMessage m, object r)
        {
            m.ResponsePort.PostUnknownType(r);
        }

        private void PassResultOnToNextStage(StageMessage m, object r)
        {
            base.Next.Post(new StageMessage
            {
                Message = r,
                ResponsePort = m.ResponsePort
            });
        }

        private void ProcessMessageAndReturnResult(Action<TInput, Port<TOutput>> handler, TInput msg, Port<TOutput> responsePort)
        {
            handler(msg, responsePort);
        }
    }
}
