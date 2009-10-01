using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Config
{
    public class CcrsRequestSingleResponseListenerConfig<TRequest, TResponse>
    {
        public Func<TRequest, TResponse> MessageHandler;
        public bool ProcessSequentially;
        public DispatcherQueue TaskQueue;
    }
}
