using System;
using CcrSpaces.Channels;

namespace CcrSpaces.Flows.Stages
{
    internal class TerminalStage<TInput> : StageBase
    {
        public TerminalStage(Action<TInput> handler)
        {
            base.Configure(new CcrsOneWayChannelConfig<StageMessage>
                                {
                                    MessageHandler = m => ConsumeMessage(handler, (TInput)m.Message),
                                });
        }


        private void ConsumeMessage(Action<TInput> handler, TInput msg)
        {
            handler(msg);
        }
    }
}
