using System;
using CcrSpaces.Core.Channels;

namespace CcrSpaces.Core.Flows.Stages
{
    internal class TerminalStage<TInput> : StageBase
    {
        public TerminalStage(CcrsOneWayChannelConfig<TInput> config)
        {
            base.Configure(new CcrsOneWayChannelConfig<StageMessage>
                               {
                                   MessageHandler = m => ConsumeMessage(config.MessageHandler, (TInput)m.Message),
                                   TaskQueue = config.TaskQueue,
                                   HandlerMode = config.HandlerMode
                               });
        }


        private void ConsumeMessage(Action<TInput> handler, TInput msg)
        {
            handler(msg);
        }
    }
}
