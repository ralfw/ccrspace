using System;
using CcrSpace.Core;
using Microsoft.Ccr.Core;

namespace CcrSpace.Channels
{
    public static class CcrsChannelFactoryExtensions
    {
        public static Port<T> CreateChannel<T>(this ICcrSpace space, Action<T> messageHandler)
        {
            return CreateChannel(space, messageHandler, CcrsChannelHandlerModes.Sequential);
        }

        public static Port<T> CreateChannel<T>(this ICcrSpace space, Action<T> messageHandler, CcrsChannelHandlerModes handlerMode)
        {
            return CreateChannel(space, new CcrsChannelConfig<T>
                                            {
                                                MessageHandler = messageHandler,
                                                HandlerMode = handlerMode
                                            });
        }

        public static Port<T> CreateChannel<T>(this ICcrSpace space, CcrsChannelConfig<T> config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return ChannelFactory.Instance.CreateChannel(config);
        }
    }
}
