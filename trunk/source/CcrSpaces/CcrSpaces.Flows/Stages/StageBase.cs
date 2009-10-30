using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows.Stages
{
    internal class StageMessage
    {
        public object Message;
        public IPort ResponsePort;
    }


    internal class StageBase : Port<StageMessage>
    {
        public StageBase Next;

        protected void Configure(CcrsOneWayChannelConfig<StageMessage> config)
        {
            new CcrsChannelFactory()
                .ConfigureChannel(this, config);
        }
    }
}
