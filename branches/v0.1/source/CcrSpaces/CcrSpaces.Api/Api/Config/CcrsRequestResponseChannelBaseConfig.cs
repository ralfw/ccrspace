using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Config
{
    public class CcrsRequestResponseChannelBaseConfig<TRequest, TResponse>
    {
        public class Request
        {
            public TRequest Message;
            public ICcrsSimplexChannel<TResponse> Response;
        }

        public Action<Request> MessageHandler;
        public bool ProcessSequentially;
        public DispatcherQueue TaskQueue;
    }
}
