using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api
{
    public class CcrsFlow<TRequest> : ICcrsSimplexChannel<TRequest>
    {
        public void Post(TRequest message)
        { }

        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            throw new NotImplementedException();
        }

        public bool TryPostUnknownType(object item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CcrsFlow<TRequest, TOutput> : ICcrsDuplexChannel<TRequest, TOutput>
    {
        public void Post(TRequest message)
        { }

        public void Post(TRequest request, Action<TOutput> responseHandler)
        { }

        public void Post(TRequest request, ICcrsSimplexChannel<TOutput> responseSimplexChannel)
        { }


        public CcrsFlow<TRequest> Concat(ICcrsSimplexChannel<TOutput> outputHandler)
        {
            return new CcrsFlow<TRequest>();
        }

        public CcrsFlow<TRequest, TNextOutput> Concat<TNextOutput>(ICcrsDuplexChannel<TOutput, TNextOutput> outputHandler)
        {
            return new CcrsFlow<TRequest, TNextOutput>();
        }

        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            throw new NotImplementedException();
        }

        public bool TryPostUnknownType(object item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
