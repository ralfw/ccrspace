using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    public class CcrsPendingRequest<TInput, TOutput>
    {
        internal PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType> Requests;
        internal TInput Request;


        public void Receive(Action<TOutput> responseHandler)
        {
            this.Receive(new CcrsChannelFactory().CreateChannel(new CcrsOneWayChannelConfig<TOutput> { MessageHandler = responseHandler }));
        }

        public void Receive(Action<TOutput> responseHandler, CcrsChannelHandlerModes handlerMode)
        {
            this.Receive(new CcrsChannelFactory().CreateChannel(new CcrsOneWayChannelConfig<TOutput> { MessageHandler = responseHandler, HandlerMode = handlerMode }));
        }

        public void Receive(Port<TOutput> responsePort)
        {
            this.Requests.Post(new CcrsRequest<TInput, TOutput>(this.Request, responsePort));
        }
    }
}
