using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Flows
{
    public abstract class CcrsFlowBase
    {
        protected readonly List<ICcrsDuplexChannel> intermediateStages;


        protected CcrsFlowBase(CcrsRequestResponseFlowConfig cfg)
        {
            this.intermediateStages = new List<ICcrsDuplexChannel>(cfg.IntermediateStages);
        }


        protected Action<object> ContinueWithUnknownType(int stageIndex, IPort finalStage)
        {
            if (stageIndex > this.intermediateStages.Count - 1)
                return finalStage.PostUnknownType;

            Action<object, Action<object>> post = this.intermediateStages[stageIndex].PostUnknownType;

            if (stageIndex == this.intermediateStages.Count - 1)
                return msg => post(msg, finalStage.PostUnknownType);

            return msg => post(msg, r => ContinueWithUnknownType(stageIndex + 1, finalStage)(r));
        }
    }
}
