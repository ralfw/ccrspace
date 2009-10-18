using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    public enum CcrsChannelHandlerModes
    {
        Parallel,
        Sequential,
        InCurrentSyncContext
    }


    public class CcrsChannelConfig<T>
    {
        public Action<T> MessageHandler;
        public DispatcherQueue TaskQueue;
        public CcrsChannelHandlerModes HandlerMode = CcrsChannelHandlerModes.Sequential;
    }


    public class CcrsChannelConfig<TInput, TOutput>
    {
        public Action<TInput, Port<TOutput>> InputMessageHandler;
        public Action<TOutput> OutputMessageHandler;

        public DispatcherQueue TaskQueue;
        public CcrsChannelHandlerModes InputHandlerMode = CcrsChannelHandlerModes.Sequential;
        public CcrsChannelHandlerModes OutputHandlerMode = CcrsChannelHandlerModes.Sequential;
    }
}
