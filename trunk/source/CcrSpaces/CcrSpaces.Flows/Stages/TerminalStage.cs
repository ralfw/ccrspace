using System;
using CcrSpaces.Channels;

namespace CcrSpaces.Flows.Stages
{
    internal class TerminalStage<TInput> : StageBase
    {
        public TerminalStage(Action<TInput> handler)
            : this(new CcrsOneWayChannelConfig<TInput>{MessageHandler=handler})
        {}

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
