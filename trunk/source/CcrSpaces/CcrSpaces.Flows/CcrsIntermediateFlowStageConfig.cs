using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Flows
{
    [Flags]
    public enum CcrsFlowStageFlags
    {
        IndividualTaskQueue=1
    }


    public class CcrsFlowConfig
    {
        internal CcrsFlowConfig(){}

        public DispatcherQueue TaskQueue;
        public CcrsHandlerModes HandlerMode = CcrsHandlerModes.Sequential;

        public CcrsFlowStageFlags StageFlags = CcrsFlowStageFlags.IndividualTaskQueue;
    }

    public class CcrsIntermediateFlowStageConfig<TInput, TOutput> : CcrsFlowConfig
    {
        public Action<TInput, Port<TOutput>> InputMessageHandler;
    }
}
