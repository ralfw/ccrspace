using System;
using CcrSpaces.Core.Flows;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public static class CcrSpaceExtensionsForFlows
    {
        public static CcrsFlow<TInput, TOutput> StartFlow<TInput, TOutput>(this ICcrSpace space, Func<TInput, TOutput> handler)
        { return space.StartFlow<TInput, TOutput>((m, pr) => pr.Post(handler(m))); }
        public static CcrsFlow<TInput, TOutput> StartFlow<TInput, TOutput>(this ICcrSpace space, Action<TInput, Port<TOutput>> handler)
        {
            return space.StartFlow(new CcrsIntermediateFlowStageConfig<TInput, TOutput>
                                       {
                                           InputMessageHandler = handler,
                                           TaskQueue = space.DefaultTaskQueue
                                       });
        }
        public static CcrsFlow<TInput, TOutput> StartFlow<TInput, TOutput>(this ICcrSpace space, CcrsIntermediateFlowStageConfig<TInput, TOutput> config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return new CcrsFlow<TInput, TOutput>(config);
        }
    }
}
