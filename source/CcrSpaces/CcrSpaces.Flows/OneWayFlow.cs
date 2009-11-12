using System;
using CcrSpaces.Core.Channels.Extensions;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Flows.Stages;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Flows
{
    public class CcrsFlow<TInput> : Port<TInput>
    {
        private readonly StageBase firstStage;


        internal CcrsFlow(StageBase firstStage)
        {
            this.firstStage = firstStage;
            this.RegisterGenericSyncReceiver(msg => this.firstStage.Post(new StageMessage { Message = msg }));
        }

        public CcrsFlow(Action<TInput> handler)
            : this(new CcrsOneWayChannelConfig<TInput>{MessageHandler = handler}) {}
        public CcrsFlow(CcrsOneWayChannelConfig<TInput> config)
        {
            this.firstStage = new TerminalStage<TInput>(config);
            this.RegisterGenericSyncReceiver(msg => this.firstStage.Post(new StageMessage { Message = msg }));
        }
    }
}
