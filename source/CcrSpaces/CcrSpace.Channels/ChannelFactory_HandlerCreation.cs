using System;
using System.Threading;
using Microsoft.Ccr.Core;
using CcrSpaces.Channels.Extensions;

namespace CcrSpaces.Channels
{
    partial class CcrsChannelFactory
    {
        private Action<T> CreateInSynContextHandler<T>(CcrsChannelConfig<T> config)
        {
            Action<T> safeHandler = config.MessageHandler;
            if (config.HandlerMode == CcrsChannelHandlerModes.InCurrentSyncContext)
            {
                SynchronizationContext currentContext = SynchronizationContext.Current;
                safeHandler = m =>
                                    {
                                        currentContext.Send(
                                            delegate { config.MessageHandler(m); },
                                            null
                                            );
                                    };
            }
            return safeHandler;
        }


        private void CreateSequentialHandler<T>(CcrsChannelConfig<T> config, Action<T> safeHandler, Port<T> port)
        {
            Action<T> sequentialHandler = null;
            sequentialHandler = m =>
                                    {
                                        safeHandler(m);
// ReSharper disable AccessToModifiedClosure
                                        port.WireUpHandler(config.TaskQueue, false, sequentialHandler);
// ReSharper restore AccessToModifiedClosure
                                    };
            port.WireUpHandler(config.TaskQueue, false, sequentialHandler);
        }


        private void CreateParallelHandler<T>(CcrsChannelConfig<T> config, Port<T> port)
        {
            port.WireUpHandler(config.TaskQueue, true, config.MessageHandler);
        }
    }
}
