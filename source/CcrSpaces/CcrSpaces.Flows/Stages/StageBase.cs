using CcrSpaces.Core.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Flows.Stages
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
