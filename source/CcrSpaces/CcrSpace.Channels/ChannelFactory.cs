using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    internal interface IChannelFactory
    {
        Port<T> CreateChannel<T>(CcrsChannelConfig<T> config);
    }

    internal partial class ChannelFactory : IChannelFactory
    {
        private static IChannelFactory instance;
        public static IChannelFactory Instance
        {
            get
            {
                if (instance == null) instance = new ChannelFactory();
                return instance;
            }
            set
            {
                instance = value;
            }
        }


        public Port<T> CreateChannel<T>(CcrsChannelConfig<T> config)
        {
            var port = new Port<T>();
            {
                if (config.HandlerMode == CcrsChannelHandlerModes.Sequential || config.HandlerMode == CcrsChannelHandlerModes.InCreatorSyncContext)
                {
                    Action<T> safeHandler = CreateInSynContextHandler(config);
                    CreateSequentialHandler(config, safeHandler, port);
                }
                else
                    CreateParallelHandler(config, port);
            }
            return port;
        }
    }
}
