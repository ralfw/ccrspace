using System;
using System.Threading;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    partial class ChannelFactory
    {
        private Action<T> CreateInSynContextHandler<T>(CcrsChannelConfig<T> config)
        {
            Action<T> safeHandler = config.MessageHandler;
            if (config.HandlerMode == CcrsChannelHandlerModes.InCreatorSyncContext)
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
                                        Register(port, config.TaskQueue, false, sequentialHandler);
                                    };
            Register(port, config.TaskQueue, false, sequentialHandler);
        }


        private void CreateParallelHandler<T>(CcrsChannelConfig<T> config, Port<T> port)
        {
            Register(port, config.TaskQueue, true, config.MessageHandler);
        }
    }
}
