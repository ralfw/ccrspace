using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api
{
    public class CcrsOneWayListener<TMessage> : ICcrsSimplexChannel<TMessage>
    {
        internal CcrsOneWayListener()
        {}

        public CcrsOneWayListener(Action<TMessage> messageHandler)
        {}


        public void Post(TMessage message)
        { }
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
