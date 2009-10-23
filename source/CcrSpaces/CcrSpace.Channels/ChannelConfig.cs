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


    public class CcrsOneWayChannelConfig<T>
    {
        public Action<T> MessageHandler;
        public DispatcherQueue TaskQueue;
        public CcrsChannelHandlerModes HandlerMode = CcrsChannelHandlerModes.Sequential;
    }


    public class CcrsFilterChannelConfig<TInput, TOutput>
    {
        public Action<TInput, Port<TOutput>> InputMessageHandler;
        public Port<TOutput> OutputPort;

        public DispatcherQueue TaskQueue;
        public CcrsChannelHandlerModes InputHandlerMode = CcrsChannelHandlerModes.Sequential;
    }

    
    public class CcrsRequestResponseChannelConfig<TInput, TOutput>
    {
        public Action<TInput, Port<TOutput>> InputMessageHandler;
        public Action<TOutput> OutputMessageHandler;

        public DispatcherQueue TaskQueue;
        public CcrsChannelHandlerModes InputHandlerMode = CcrsChannelHandlerModes.Sequential;
        public CcrsChannelHandlerModes OutputHandlerMode = CcrsChannelHandlerModes.Sequential;
    }
}
