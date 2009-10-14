using System;
using System.Threading;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    partial class ChannelFactory
    {
        private void ConfigurePort<T>(Port<T> port, CcrsChannelConfig<T> config)
        {
            if (config.HandlerMode == CcrsChannelHandlerModes.Sequential || config.HandlerMode == CcrsChannelHandlerModes.InCurrentSyncContext)
            {
                Action<T> safeHandler = CreateInSynContextHandler(config);
                CreateSequentialHandler(config, safeHandler, port);
            }
            else
                CreateParallelHandler(config, port);
        }


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
                                        Register(port, config.TaskQueue, false, sequentialHandler);
// ReSharper restore AccessToModifiedClosure
                                    };
            Register(port, config.TaskQueue, false, sequentialHandler);
        }


        private void CreateParallelHandler<T>(CcrsChannelConfig<T> config, Port<T> port)
        {
            Register(port, config.TaskQueue, true, config.MessageHandler);
        }
    }
}
