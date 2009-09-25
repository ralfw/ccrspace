using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api
{
    public interface ICcrsSimplexChannel<TMessage>
    {
        void Post(TMessage message);
    }


    public interface ICcrsDuplexChannel<TRequest, TResponse> : ICcrsSimplexChannel<TRequest>
    {
        void Post(TRequest request, Action<TResponse> responseHandler);
        void Post(TRequest request, ICcrsSimplexChannel<TResponse> responseSimplexChannel);
    }


}
