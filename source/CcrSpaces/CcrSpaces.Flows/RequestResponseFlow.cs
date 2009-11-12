using System;
using CcrSpaces.Core.Channels.Extensions;
using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Flows.Stages;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Flows
{
    public class CcrsFlow<TInput, TOutput> : Port<CcrsRequest<TInput, TOutput>>
    {
        private readonly CcrsFlowConfig overallFlowConfig;

        private readonly StageBase firstStage;
        private StageBase lastStage;
        


        internal CcrsFlow(CcrsFlowConfig config, StageBase firstStage, StageBase lastStage)
        {
            this.overallFlowConfig = config;
            this.firstStage = firstStage;
            this.lastStage = lastStage;
            WireUpRequestPort();
        }


        public CcrsFlow(Action<TInput, Port<TOutput>> handler)
            : this(new CcrsIntermediateFlowStageConfig<TInput, TOutput> { InputMessageHandler = handler })
        {}
        public CcrsFlow(CcrsIntermediateFlowStageConfig<TInput, TOutput> config)
        {
            this.overallFlowConfig = config;
            this.firstStage = new IntermediateStage<TInput, TOutput>(config);
            this.lastStage = this.firstStage;
            WireUpRequestPort();
        }


        private void WireUpRequestPort()
        {
            this.RegisterGenericSyncReceiver(msg =>
                {
                    var req = (CcrsRequest<TInput, TOutput>)msg;
                    this.firstStage.Post(new StageMessage { Message = req.Request, ResponsePort = req.Responses });
                });
        }


        public CcrsFlow<TInput, TNextOutput> Continue<TNextOutput>(Func<TOutput, TNextOutput> intermediateHandler)
        { return Continue<TNextOutput>((m, p) => p.Post(intermediateHandler(m))); }
        public CcrsFlow<TInput, TNextOutput> Continue<TNextOutput>(Action<TOutput, Port<TNextOutput>> intermediateHandler)
        { return Continue(new CcrsIntermediateFlowStageConfig<TOutput, TNextOutput> { InputMessageHandler = intermediateHandler }); }
        public CcrsFlow<TInput, TNextOutput> Continue<TNextOutput>(CcrsIntermediateFlowStageConfig<TOutput, TNextOutput> config)
        {
            config.TaskQueue = GetTaskQueueForStage(config.TaskQueue);

            this.lastStage.Next = new IntermediateStage<TOutput, TNextOutput>(config);
            this.lastStage = this.lastStage.Next;
            return new CcrsFlow<TInput, TNextOutput>(this.overallFlowConfig, this.firstStage, this.lastStage);
        }


        public CcrsFlow<TInput> Terminate(Action<TOutput> terminalHandler)
        { return Terminate(new CcrsOneWayChannelConfig<TOutput>{MessageHandler=terminalHandler}); }
        public CcrsFlow<TInput> Terminate(CcrsOneWayChannelConfig<TOutput> config)
        {
            config.TaskQueue = GetTaskQueueForStage(config.TaskQueue);

            this.lastStage.Next = new TerminalStage<TOutput>(config);
            return new CcrsFlow<TInput>(this.firstStage);
        }

        
        private DispatcherQueue GetTaskQueueForStage(DispatcherQueue stageTaskQueue)
        {
            if (stageTaskQueue == null && (this.overallFlowConfig.StageFlags & CcrsFlowStageFlags.IndividualTaskQueue) == CcrsFlowStageFlags.IndividualTaskQueue)
                return this.overallFlowConfig.TaskQueue != null
                               ? new DispatcherQueue("FlowStage" + Guid.NewGuid(), this.overallFlowConfig.TaskQueue.Dispatcher)
                               : new DispatcherQueue();

            return stageTaskQueue;
        }
    }
}
