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
    internal class CcrsFlow<TInput> : Port<TInput>
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
