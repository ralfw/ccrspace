using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows.Infrastructure
{
    internal static partial class IPortReceiveExtensions
    {
        public static void RegisterGenericSyncReceiver(this IPortReceive port, Action<object> elementHandler)
        {
            port.RegisterReceiver(new ConsumingReceiverTask(elementHandler));
        }
    }
}
