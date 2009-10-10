using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;

namespace CcrSpaces.Api.Flows
{
    public class CcrsFlow<TRequest> : CcrsFlowBase, ICcrsSimplexChannel<TRequest>
    {
        private readonly ICcrsSimplexChannel finalStage;


        public CcrsFlow(CcrsOneWayFlowConfig cfg)
            : base(cfg)
        {
            this.finalStage = cfg.FinalStage;
        }


        public void Post(TRequest message)
        {
            if (this.finalStage == null) throw new InvalidOperationException("Missing a simple channel for the final stage of the flow!");

            this.intermediateStages[0].PostUnknownType(message, r => ContinueWithUnknownType(1, this.finalStage)(r));
        }


        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            this.Post((TRequest)item);
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
