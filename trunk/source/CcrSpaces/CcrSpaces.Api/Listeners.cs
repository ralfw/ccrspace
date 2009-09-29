using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsOneWayListener<TMessage> : ICcrsSimplexChannel<TMessage>
    {
        private Port<TMessage> channel;


        internal CcrsOneWayListener() {}
        public CcrsOneWayListener(Action<TMessage> messageHandler)
        {
            this.channel = new Port<TMessage>();
            Arbiter.Activate(
                new DispatcherQueue(),
                Arbiter.Receive(
                    true,
                    this.channel,
                    new Handler<TMessage>(messageHandler)
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
