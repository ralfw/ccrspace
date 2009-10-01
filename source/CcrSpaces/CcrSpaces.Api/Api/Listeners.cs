using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsOneWayListener<TMessage> : ICcrsSimplexChannel<TMessage>
    {
        private readonly Port<TMessage> channel;


        public CcrsOneWayListener(Action<TMessage> messageHandler)
            : this(new CcrsListenerConfig<TMessage> { MessageHandler = messageHandler, ProcessSequentially=false })
        { }
        
        internal CcrsOneWayListener(CcrsListenerConfig<TMessage> cfg)
        {
            this.channel = new Port<TMessage>();

            if (cfg.ProcessSequentially)
            {
                Handler<TMessage> messageHandler = null;
                messageHandler = new Handler<TMessage>(m =>
                                                           {
                                                               cfg.MessageHandler(m); 
                                                               Receive(false, messageHandler);
                                                           });
                Receive(false, messageHandler);
            }
            else
                Receive(true, new Handler<TMessage>(cfg.MessageHandler));
        }


        private void Receive(bool persistentPortBinding, Handler<TMessage> messageHandler)
        {
            Arbiter.Activate(
                new DispatcherQueue(),
                Arbiter.Receive(
                    persistentPortBinding,
                    this.channel,
                    messageHandler
                    )
                );
        }


        public void Post(TMessage message)
        {
            this.channel.Post(message);
        }
    }


    public class CcrsRequestResponseListener<TRequest, TResponse> : ICcrsDuplexChannel<TRequest, TResponse>
    {
        public void Post(TRequest message)
        { }

        public void Post(TRequest request, Action<TResponse> responseHandler)
        { }

        public void Post(TRequest request, ICcrsSimplexChannel<TResponse> responseSimplexChannel)
        { }



        public CcrsFlow<TRequest> Concat(ICcrsSimplexChannel<TResponse> responseHandler)
        {
            return new CcrsFlow<TRequest>();
        }

        public CcrsFlow<TRequest, TOutput> Concat<TOutput>(ICcrsDuplexChannel<TResponse, TOutput> responseHandler)
        {
            return new CcrsFlow<TRequest, TOutput>();
        }
    }
}
