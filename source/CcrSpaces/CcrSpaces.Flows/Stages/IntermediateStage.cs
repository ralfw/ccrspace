using CcrSpaces.Core.Channels.Extensions;
using CcrSpaces.Core.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Flows.Stages
{
    internal partial class IntermediateStage<TInput, TOutput> : StageBase
    {
        public IntermediateStage(CcrsIntermediateFlowStageConfig<TInput, TOutput> config)
        {
            base.Configure(new CcrsOneWayChannelConfig<StageMessage>
                               {
                                   MessageHandler = m =>
                                       {
                                           AssertHasResponsePort(m);
                                           Port<TOutput> responseInterceptor = CreateLocalResponsePort(m);
                                           ProcessMessageAndReturnResult(config.InputMessageHandler, (TInput)m.Message, responseInterceptor);
                                       },
                                   TaskQueue = config.TaskQueue,
                                   HandlerMode = config.HandlerMode
                               });
        }


        private Port<TOutput> CreateLocalResponsePort(StageMessage m)
        {
            var responseInterceptor = new Port<TOutput>();
            responseInterceptor.RegisterGenericSyncReceiver(r =>
                    {
                        if (IsTerminalStage())
                            SendResultBackToCaller(m, r);
                        else
                            PassResultOnToNextStage(m, r);

                    });
            return responseInterceptor;
        }
    }
}
