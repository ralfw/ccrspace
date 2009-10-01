using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Infrastructure
{
    internal static class PortExtensions
    {
        public static void RegisterHandler<T>(this Port<T> port, Action<T> handler, DispatcherQueue taskQueue, bool processSequentially)
        {
            if (processSequentially)
            {
                Action<T> sequentialHandler = null;
                sequentialHandler = m =>
                                        {
                                            handler(m);
                                            Register(port, taskQueue, false, sequentialHandler);
                                        };
                Register(port, taskQueue, false, sequentialHandler);
            }
            else
                Register(port, taskQueue, true, handler);
        }


        private static void Register<T>(Port<T> port, DispatcherQueue taskQueue, bool persistent, Action<T> handler)
        {
            Arbiter.Activate(
                taskQueue,
                Arbiter.Receive(
                    persistent,
                    port,
                    new Handler<T>(handler)
                    )
                );
        }
    }
}
