using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsRequestResponseListener<TRequest, TResponse> : ICcrsDuplexChannel<TRequest, TResponse>
    {
        private class Request
        {
            public TRequest Message;
            public ICcrsSimplexChannel<TResponse> Response;
        }

        private readonly Port<Request> channel;
        private readonly DispatcherQueue taskQueue;


        public CcrsRequestResponseListener(Func<TRequest, TResponse> messageHandler) :
            this(new CcrsRequestResponseListenerConfig<TRequest, TResponse>{MessageHandler=messageHandler, TaskQueue=new DispatcherQueue(), ProcessSequentially=false})
        {}

        public CcrsRequestResponseListener(CcrsRequestResponseListenerConfig<TRequest, TResponse> cfg)
        {
            this.taskQueue = cfg.TaskQueue;

            this.channel = new Port<Request>();
            this.channel.RegisterHandler(req =>
                                            {
                                                var resp = cfg.MessageHandler(req.Message);
                                                req.Response.Post(resp);
                                            },
                                            this.taskQueue,
                                            cfg.ProcessSequentially);
        }


        public void Post(TRequest message) { this.Post(message, resp => { }); }
        public void Post(TRequest message, Action<TResponse> responseHandler) { this.Post(message, new CcrsOneWayListener<TResponse>(responseHandler, this.taskQueue)); }
        public void Post(TRequest message, ICcrsSimplexChannel<TResponse> responseSimplexChannel)
        {
            var req = new Request {Message=message, Response=responseSimplexChannel};
            this.channel.Post(req);
        }
    }
}
