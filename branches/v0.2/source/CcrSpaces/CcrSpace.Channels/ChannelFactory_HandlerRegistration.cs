using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    partial class ChannelFactory
    {
        private static void Register<T>(Port<T> port, DispatcherQueue taskQueue, bool persistent, Action<T> handler)
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
