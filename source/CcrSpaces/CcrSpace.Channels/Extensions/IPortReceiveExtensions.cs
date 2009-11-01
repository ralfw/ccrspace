using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels.Extensions
{
    public static partial class IPortReceiveExtensions
    {
        public static void RegisterGenericSyncReceiver(this IPortReceive port, Action<object> elementHandler)
        {
            port.RegisterReceiver(new ConsumingReceiverTask(elementHandler));
        }
    }
}
