using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Channels.Extensions
{
    public static class PortExtensions
    {
        public static void WireUpHandler<T>(this Port<T> port, DispatcherQueue taskQueue, bool persistent, Action<T> handler)
        {
            Arbiter.Activate(
                taskQueue ?? new DispatcherQueue(),
                Arbiter.Receive(
                    persistent,
                    port,
                    new Handler<T>(handler)
                    )
                );
        }
    }
}
