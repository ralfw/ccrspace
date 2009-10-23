using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;
using CcrSpaces.Flows.Infrastructure;

namespace CcrSpaces.Flows
{
    class CcrsFlow<TInput, TOutput> : PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType>
    {
        private readonly List<Port<CcrsRequestOfUnknownType>> stages;


        internal CcrsFlow(IEnumerable<Port<CcrsRequestOfUnknownType>> stages)
        {
            this.stages = new List<Port<CcrsRequestOfUnknownType>>(stages);
            ConnectStages();
        }


        public CcrsFlow<TInput, TNewOutput> Continue<TNewOutput>(Func<TOutput, TNewOutput> dataHandler)
        {
            return new CcrsFlow<TInput, TNewOutput>(
                this.stages.Concat(new[] {new CcrsChannelFactory()
                                                .CreateChannel(new CcrsRequestResponseChannelConfig<TOutput, TNewOutput>
                                                                {
                                                                    InputMessageHandler = (e, p) => p.Post(dataHandler(e)),
                                                                    InputHandlerMode = CcrsChannelHandlerModes.Sequential
                                                                }).P2}));
        }


        private void ConnectStages()
        {
            // input port an erste stage weiterleiten
            this.P0.RegisterGenericSyncReceiver(msg => this.stages[0].Post(new CcrsRequestOfUnknownType(msg, null)));
            this.P1.RegisterGenericSyncReceiver(req =>
                                                    {
                                                        CcrsRequestOfUnknownType reqOfUnknownType = ((ICcrsConverterToRequestOfUnknownType)req).ToRequestOfUnknownType();
                                                        this.stages[0].Post(reqOfUnknownType);
                                                    });
            this.P2.RegisterGenericSyncReceiver(reqOfUnknownType => this.stages[0].Post((CcrsRequestOfUnknownType)reqOfUnknownType));

            // stages verbinden
            // letzte stage an output port weiterleiten
        }


        //public CcrsFlow<TInput> Finish(Action<TOutput> dataHandler)
        //{
        //    return null;
        //}
    }
}
 