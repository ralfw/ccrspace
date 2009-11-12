using System;
using CcrSpaces.Core.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public class CcrsPendingRequest<TInput, TOutput>
    {
        internal Port<CcrsRequest<TInput, TOutput>> Requests;
        internal TInput Request;


        public void Receive(Action<TOutput> responseHandler)
        {
            this.Receive(new CcrsChannelFactory().CreateChannel(new CcrsOneWayChannelConfig<TOutput> { MessageHandler = responseHandler }));
        }

        public void Receive(Action<TOutput> responseHandler, CcrsHandlerModes handlerMode)
        {
            this.Receive(new CcrsChannelFactory().CreateChannel(new CcrsOneWayChannelConfig<TOutput> { MessageHandler = responseHandler, HandlerMode = handlerMode }));
        }

        public void Receive(Port<TOutput> responsePort)
        {
            this.Requests.Post(new CcrsRequest<TInput, TOutput>(this.Request, responsePort));
        }
    }
}
