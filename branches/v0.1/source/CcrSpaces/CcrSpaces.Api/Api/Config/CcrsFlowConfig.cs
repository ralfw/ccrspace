using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    public class CcrsRequestResponseFlowConfig
    {
        public List<ICcrsDuplexChannel> IntermediateStages = new List<ICcrsDuplexChannel>();

        public void AddStage(ICcrsDuplexChannel stage) { this.IntermediateStages.Add(stage); }
    }


    public class CcrsOneWayFlowConfig : CcrsRequestResponseFlowConfig
    {
        public ICcrsSimplexChannel FinalStage;

        public void AddStage(ICcrsSimplexChannel finalStage) { this.FinalStage = finalStage; }
    }
}
