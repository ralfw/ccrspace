using System;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Extensions
{
    public static class ChannelExtensions
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
    }
}
