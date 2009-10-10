using System;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Flows
{
    public class CcrsFlow<TRequest, TResponse> : CcrsFlowBase, ICcrsDuplexChannel<TRequest, TResponse>
    {
        public CcrsFlow(CcrsRequestResponseFlowConfig cfg)
            : base(cfg)
        { }


        public void Post(TRequest request, Action<TResponse> finalStageHandler)
        {
            this.Post(
                request,
                new CcrsOneWayChannel<TResponse>(new CcrsOneWayChannelConfig<TResponse>
                    {
                        MessageHandler = finalStageHandler,
                        TaskQueue = new DispatcherQueue() //this.taskQueue
                    })
                );
        }

        public void Post(TRequest request, ICcrsSimplexChannel<TResponse> finalStage)
        {
            this.intermediateStages[0].PostUnknownType(request, r => ContinueWithUnknownType(1, finalStage)(r));
        }


        public void PostUnknownType(object message, Action<object> responseHandler)
        {
            this.Post((TRequest)message, o => responseHandler(o));
        }


        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            this.Post((TRequest)item, o => { });
        }

        public bool TryPostUnknownType(object item)
        {
            if (!(item is TRequest)) return false;

            this.PostUnknownType(item);
            return true;
        }

        #endregion
    }
}
