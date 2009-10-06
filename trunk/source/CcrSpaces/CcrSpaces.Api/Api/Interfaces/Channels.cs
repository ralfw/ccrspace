using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public interface ICcrsSimplexChannel : IPort
    {}

    public interface ICcrsSimplexChannel<TMessage> : ICcrsSimplexChannel
    {
        void Post(TMessage message);
    }


    public interface ICcrsDuplexChannel
    {
        void PostUnknownType(object request, Action<object> responseHandler);
    }

    public interface ICcrsDuplexChannel<TRequest, TResponse> : ICcrsDuplexChannel
    {
        void Post(TRequest request, Action<TResponse> responseHandler);
        void Post(TRequest request, ICcrsSimplexChannel<TResponse> responseSimplexChannel);
    }
}
