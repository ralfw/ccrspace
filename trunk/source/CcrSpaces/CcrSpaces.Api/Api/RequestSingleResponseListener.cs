using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsRequestSingleResponseListener<TRequest, TResponse> : CcrsRequestResponseListenerBase<TRequest, TResponse>
    {
        public CcrsRequestSingleResponseListener(Func<TRequest, TResponse> messageHandler) :
            this(new CcrsRequestSingleResponseListenerConfig<TRequest, TResponse>{MessageHandler=messageHandler, TaskQueue=new DispatcherQueue(), ProcessSequentially=false})
        {}
        public CcrsRequestSingleResponseListener(CcrsRequestSingleResponseListenerConfig<TRequest, TResponse> cfg) :
            base(new CcrsRequestResponseListenerConfig<TRequest,TResponse>
                     {
                         MessageHandler = req =>
                                             {
                                                 var resp = cfg.MessageHandler(req.Message);
                                                 req.Response.Post(resp);
                                             },
                         TaskQueue = cfg.TaskQueue,
                         ProcessSequentially = cfg.ProcessSequentially
                     })
        {}
    }
}
