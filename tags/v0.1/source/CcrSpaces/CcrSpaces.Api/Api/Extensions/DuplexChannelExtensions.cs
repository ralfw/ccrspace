using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Api.Config.Fluent;
using CcrSpaces.Api.Flows;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Extensions
{
    public static class DuplexChannelExtensions
    {
        public static void Post<TRequest, TResponse>(this ICcrsDuplexChannel<TRequest, TResponse> channel, TRequest request, Action<TResponse> responseHandler, bool processInCurrentSyncContext)
        {
            channel.Post(request, new CcrsOneWayChannel<TResponse>(new CcrsOneWayChannelConfig<TResponse>
                                                                       {
                                                                           MessageHandler = responseHandler,
                                                                           ProcessSequentially = processInCurrentSyncContext,
                                                                           ProcessInCurrentSyncContext = processInCurrentSyncContext,
                                                                           TaskQueue = new DispatcherQueue()
                                                                       }));
        }


        public static CcrsFlow<TRequest> Do<TRequest>(this ICcrsDuplexChannel<TRequest> headStage, ICcrsSimplexChannel finalStage)
        {
            return new CcrsFlowFluent<TRequest>().Do(headStage).Do(finalStage);
        }

        public static CcrsFlowFluent<TRequest> Do<TRequest>(this ICcrsDuplexChannel<TRequest> headStage, ICcrsDuplexChannel nextStage)
        {
            return new CcrsFlowFluent<TRequest>().Do(headStage).Do(nextStage);
        }


        public static CcrsFlow<TInput, TOutput> Do<TInput, TOutput>(this ICcrsDuplexChannel<TInput> headStage, ICcrsDuplexChannel nextStage)
        {
            return new CcrsFlowFluent<TInput, TOutput>().Do(headStage).Do(nextStage);
        }
    }
}
