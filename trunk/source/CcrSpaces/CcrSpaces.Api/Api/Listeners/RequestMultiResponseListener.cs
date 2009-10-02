using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsRequestMultiResponseListener<TRequest, TResponse> : CcrsRequestResponseListenerBase<TRequest, TResponse>
    {
        public CcrsRequestMultiResponseListener(Action<TRequest, ICcrsSimplexChannel<TResponse>> messageHandler) :
            this(new CcrsRequestMultiResponseListenerConfig<TRequest, TResponse>{MessageHandler=messageHandler, TaskQueue=new DispatcherQueue(), ProcessSequentially=false})
        {}
        public CcrsRequestMultiResponseListener(CcrsRequestMultiResponseListenerConfig<TRequest, TResponse> cfg) : 
            base(new CcrsRequestResponseListenerConfig<TRequest,TResponse>
                     {
                         MessageHandler = req => cfg.MessageHandler(req.Message, req.Response),
                         TaskQueue = cfg.TaskQueue,
                         ProcessSequentially = cfg.ProcessSequentially
                     })
        {}
    }
}
