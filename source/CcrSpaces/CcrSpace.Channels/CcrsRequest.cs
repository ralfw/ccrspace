using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    [Serializable]
    public class CcrsRequest<TInput, TOutput>
    {
        public CcrsRequest(TInput request, Port<TOutput> responses)
        {
            this.Request = request;
            this.Responses = responses;
        }

        public TInput Request;
        public Port<TOutput> Responses;
    }
}
