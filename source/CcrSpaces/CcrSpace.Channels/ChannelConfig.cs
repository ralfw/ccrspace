using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public enum CcrsHandlerModes
    {
        Parallel,
        Sequential,
        InCurrentSyncContext
    }


    public class CcrsOneWayChannelConfig<T>
    {
        public Action<T> MessageHandler;
        public DispatcherQueue TaskQueue;
        public CcrsHandlerModes HandlerMode = CcrsHandlerModes.Sequential;
    }


    public class CcrsFilterChannelConfig<TInput, TOutput>
    {
        public Action<TInput, Port<TOutput>> InputMessageHandler;
        public Port<TOutput> OutputPort;

        public DispatcherQueue TaskQueue;
        public CcrsHandlerModes InputHandlerMode = CcrsHandlerModes.Sequential;
    }

    
    public class CcrsRequestResponseChannelConfig<TInput, TOutput>
    {
        public Action<TInput, Port<TOutput>> InputMessageHandler;
        public Action<TOutput> OutputMessageHandler;

        public DispatcherQueue TaskQueue;
        public CcrsHandlerModes InputHandlerMode = CcrsHandlerModes.Sequential;
        public CcrsHandlerModes OutputHandlerMode = CcrsHandlerModes.Sequential;
    }
}
