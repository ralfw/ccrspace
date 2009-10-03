using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsRequestMultiResponseChannel<TRequest, TResponse> : CcrsRequestResponseChannelBase<TRequest, TResponse>
    {
        public CcrsRequestMultiResponseChannel(Action<TRequest, ICcrsSimplexChannel<TResponse>> messageHandler) :
            this(new CcrsRequestMultiResponseChannelConfig<TRequest, TResponse>{MessageHandler=messageHandler, TaskQueue=new DispatcherQueue(), ProcessSequentially=false})
        {}
        public CcrsRequestMultiResponseChannel(CcrsRequestMultiResponseChannelConfig<TRequest, TResponse> cfg) : 
            base(new CcrsRequestResponseChannelConfig<TRequest,TResponse>
                     {
                         MessageHandler = req => cfg.MessageHandler(req.Message, req.Response),
                         TaskQueue = cfg.TaskQueue,
                         ProcessSequentially = cfg.ProcessSequentially
                     })
        {}
    }
}
