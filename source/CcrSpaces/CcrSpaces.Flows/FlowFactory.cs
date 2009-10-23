using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;

namespace CcrSpaces.Flows
{
    class CcrsFlow
    {
        public CcrsFlow<TInput, TOutput> Start<TInput, TOutput>(Func<TInput, TOutput> dataHandler)
        {
            return new CcrsFlow<TInput, TOutput>(new[]{new CcrsChannelFactory()
                                                    .CreateChannel(
                                                        new CcrsRequestResponseChannelConfig<TInput, TOutput>
                                                            {
                                                                InputMessageHandler = (e, p) => p.Post(dataHandler(e)),
                                                                InputHandlerMode = CcrsChannelHandlerModes.Sequential
                                                            }).P2});
        }
    }
}
