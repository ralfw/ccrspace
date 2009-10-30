using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using CcrSpaces.Flows.Infrastructure;
using CcrSpaces.Flows.Stages;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows
{
    internal class CcrsFlow<TInput, TOutput> : Port<CcrsRequest<TInput, TOutput>>
    {
        private readonly StageBase firstStage;
        private StageBase lastStage;


        public CcrsFlow(Action<TInput, Port<TOutput>> handler)
        {
            this.firstStage = new IntermediateStage<TInput, TOutput>(handler);
            this.lastStage = this.firstStage;
            WireUpRequestPort();
        }


        public CcrsFlow(StageBase firstStage, StageBase lastStage)
        {
            this.firstStage = firstStage;
            this.lastStage = lastStage;
            WireUpRequestPort();
        }


        private void WireUpRequestPort()
        {
            this.RegisterGenericSyncReceiver(msg =>
            {
                var req = (CcrsRequest<TInput, TOutput>)msg;
                this.firstStage.Post(new StageMessage { Message = req.Request, ResponsePort = req.Responses });
            });
        }


        public CcrsFlow<TInput, TNextOutput> Continue<TNextOutput>(Action<TOutput, Port<TNextOutput>> intermediateHandler)
        {
            this.lastStage.Next = new IntermediateStage<TOutput, TNextOutput>(intermediateHandler);
            this.lastStage = this.lastStage.Next;
            return new CcrsFlow<TInput, TNextOutput>(this.firstStage, this.lastStage);
        }


        public CcrsFlow<TInput> Finish(Action<TOutput> terminalHandler)
        {
            this.lastStage.Next = new TerminalStage<TOutput>(terminalHandler);
            return new CcrsFlow<TInput>(this.firstStage);
        }
    }
}
