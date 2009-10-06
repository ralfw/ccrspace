using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Flows;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Config.Fluent
{
    public class CcrsFlowFluent<TInput>
    {
        private readonly CcrsOneWayFlowConfig cfg = new CcrsOneWayFlowConfig();


        public CcrsFlowFluent<TInput> Do(ICcrsDuplexChannel intermediateStage)
        {
            this.cfg.AddStage(intermediateStage);
            return this;
        }


        public CcrsFlow<TInput> Do(ICcrsSimplexChannel finalStage)
        {
            this.cfg.AddStage(finalStage);
            return new CcrsFlow<TInput>(this.cfg);
        }
    }


    public class CcrsFlowFluent<TInput, TOutput>
    {
        private readonly CcrsOneWayFlowConfig cfg = new CcrsOneWayFlowConfig();


        public CcrsFlowFluent<TInput, TOutput> Do(ICcrsDuplexChannel intermediateStage)
        {
            this.cfg.AddStage(intermediateStage);
            return this;
        }


        public CcrsFlow<TInput, TOutput> Create()
        {
            return new CcrsFlow<TInput, TOutput>(this.cfg);
        }


        public static implicit operator CcrsFlow<TInput, TOutput>(CcrsFlowFluent<TInput, TOutput> source)
        {
            return source.Create();
        }
    }
}
