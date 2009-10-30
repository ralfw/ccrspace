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


        public CcrsFlow(StageBase firstStage, StageBase lastStage)
        {
            this.firstStage = firstStage;
            this.lastStage = lastStage;
            WireUpRequestPort();
        }


        public CcrsFlow(Action<TInput, Port<TOutput>> handler)
            : this(new CcrsFilterChannelConfig<TInput, TOutput>{InputMessageHandler=handler})
        {}
        public CcrsFlow(CcrsFilterChannelConfig<TInput, TOutput> config)
        {
            this.firstStage = new IntermediateStage<TInput, TOutput>(config);
            this.lastStage = this.firstStage;
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
        { return Continue(new CcrsFilterChannelConfig<TOutput, TNextOutput>{InputMessageHandler=intermediateHandler}); }
        public CcrsFlow<TInput, TNextOutput> Continue<TNextOutput>(CcrsFilterChannelConfig<TOutput, TNextOutput> config)
        {
            this.lastStage.Next = new IntermediateStage<TOutput, TNextOutput>(config);
            this.lastStage = this.lastStage.Next;
            return new CcrsFlow<TInput, TNextOutput>(this.firstStage, this.lastStage);
        }


        public CcrsFlow<TInput> Finish(Action<TOutput> terminalHandler)
        { return Finish(new CcrsOneWayChannelConfig<TOutput>{MessageHandler=terminalHandler}); }
        public CcrsFlow<TInput> Finish(CcrsOneWayChannelConfig<TOutput> config)
        {
            this.lastStage.Next = new TerminalStage<TOutput>(config);
            return new CcrsFlow<TInput>(this.firstStage);
        }
    }
}
