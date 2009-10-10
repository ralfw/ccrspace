using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Config
{
    public class CcrsOneWayChannelConfig<TMessage>
    {
        public Action<TMessage> MessageHandler;
        public DispatcherQueue TaskQueue;
        public bool ProcessSequentially;
        public bool ProcessInCurrentSyncContext;
    }
}
