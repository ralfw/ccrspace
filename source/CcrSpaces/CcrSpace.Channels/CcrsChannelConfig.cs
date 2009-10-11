using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    public enum CcrsChannelHandlerModes
    {
        Parallel,
        Sequential,
        InCreatorSyncContext
    }

    public class CcrsChannelConfig<T>
    {
        public Action<T> MessageHandler;
        public DispatcherQueue TaskQueue;
        public CcrsChannelHandlerModes HandlerMode = CcrsChannelHandlerModes.Sequential;
    }
}
