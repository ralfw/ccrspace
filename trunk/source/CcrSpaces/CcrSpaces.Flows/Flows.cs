using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Flows.Infrastructure;
using CcrSpaces.Flows.Stages;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows
{
    internal class CcrsFlow<TInput> : Port<TInput>
    {
        private readonly StageBase first;


        public CcrsFlow(Action<TInput> handler)
        {
            this.first = new TerminalStage<TInput>(handler);

            this.RegisterGenericSyncReceiver(msg => this.first.Post(new StageMessage{Message=msg}));
        }
    }
}
