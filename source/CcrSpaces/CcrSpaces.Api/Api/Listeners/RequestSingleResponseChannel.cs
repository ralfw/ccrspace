using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsRequestSingleResponseChannel<TRequest, TResponse> : CcrsRequestResponseChannelBase<TRequest, TResponse>
    {
        public CcrsRequestSingleResponseChannel(Func<TRequest, TResponse> messageHandler) :
            this(new CcrsRequestSingleResponseChannelConfig<TRequest, TResponse>{MessageHandler=messageHandler, TaskQueue=new DispatcherQueue(), ProcessSequentially=false})
        {}

        public CcrsRequestSingleResponseChannel(CcrsRequestSingleResponseChannelConfig<TRequest, TResponse> cfg) :
            base(new CcrsRequestResponseChannelBaseConfig<TRequest,TResponse>
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
