using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsRequestResponseChannelBase<TRequest, TResponse> : ICcrsDuplexChannel<TRequest, TResponse>
    {
        private readonly Port<CcrsRequestResponseChannelBaseConfig<TRequest, TResponse>.Request> channel;
        private readonly DispatcherQueue taskQueue;


        protected CcrsRequestResponseChannelBase(CcrsRequestResponseChannelBaseConfig<TRequest, TResponse> cfg)
        {
            this.taskQueue = cfg.TaskQueue;

            this.channel = new Port<CcrsRequestResponseChannelBaseConfig<TRequest, TResponse>.Request>();
            this.channel.RegisterHandler(cfg.MessageHandler, this.taskQueue, cfg.ProcessSequentially, false);
        }


        public void Post(TRequest message, Action<TResponse> responseHandler)
        {
            this.Post(
                message,
                new CcrsOneWayChannel<TResponse>(new CcrsOneWayChannelConfig<TResponse>
                                                      {
                                                          MessageHandler = responseHandler,
                                                          TaskQueue = this.taskQueue
                                                      })
                );
        }
        public void Post(TRequest message, ICcrsSimplexChannel<TResponse> responseSimplexChannel)
        {
            var req = new CcrsRequestResponseChannelBaseConfig<TRequest, TResponse>.Request
                          {
                              Message = message,
                              Response = responseSimplexChannel
                          };
            this.channel.Post(req);
        }


        public void PostUnknownType(object message, Action<object> responseHandler)
        {
            this.Post((TRequest)message, o => responseHandler(o));
        }


        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            this.Post((TRequest)item, o => {});
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
