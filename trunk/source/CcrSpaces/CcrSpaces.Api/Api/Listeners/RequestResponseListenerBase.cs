using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsRequestResponseListenerBase<TRequest, TResponse> : ICcrsDuplexChannel<TRequest, TResponse>
    {
        private readonly Port<CcrsRequestResponseListenerConfig<TRequest, TResponse>.Request> channel;
        private readonly DispatcherQueue taskQueue;


        protected CcrsRequestResponseListenerBase(CcrsRequestResponseListenerConfig<TRequest, TResponse> cfg)
        {
            this.taskQueue = cfg.TaskQueue;

            this.channel = new Port<CcrsRequestResponseListenerConfig<TRequest, TResponse>.Request>();
            this.channel.RegisterHandler(cfg.MessageHandler, this.taskQueue, cfg.ProcessSequentially);
        }


        public void Post(TRequest message) { this.Post(message, resp => { }); }
        public void Post(TRequest message, Action<TResponse> responseHandler)
        {
            this.Post(
                message,
                new CcrsOneWayListener<TResponse>(new CcrsOneWayListenerConfig<TResponse>
                                                      {
                                                          MessageHandler = responseHandler,
                                                          TaskQueue = this.taskQueue
                                                      })
                );
        }
        public void Post(TRequest message, ICcrsSimplexChannel<TResponse> responseSimplexChannel)
        {
            var req = new CcrsRequestResponseListenerConfig<TRequest, TResponse>.Request
                          {
                              Message = message,
                              Response = responseSimplexChannel
                          };
            this.channel.Post(req);
        }


        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            this.Post((TRequest)item);
        }

        public bool TryPostUnknownType(object item)
        {
            if (!(item is TRequest)) return false;
            
            this.PostUnknownType(item);
            return true;
        }

        #endregion
    }
}
