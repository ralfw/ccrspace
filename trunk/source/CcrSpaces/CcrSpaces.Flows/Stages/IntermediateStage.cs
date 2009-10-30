using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using CcrSpaces.Flows.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows.Stages
{
    internal partial class IntermediateStage<TInput, TOutput> : StageBase
    {
        public IntermediateStage(Action<TInput, Port<TOutput>> handler)
        {
            base.Configure(new CcrsOneWayChannelConfig<StageMessage>
                              {
                                  MessageHandler = m =>
                                       {
                                           AssertHasResponsePort(m);
                                           Port<TOutput> responseInterceptor = CreateLocalResponsePort(m);
                                           ProcessMessageAndReturnResult(handler, (TInput)m.Message, responseInterceptor);
                                       },
                              });
        }

        public IntermediateStage(CcrsOneWayChannelConfig<StageMessage> config) { base.Configure(config); }




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
