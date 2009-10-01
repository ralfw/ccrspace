using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrSpace : IDisposable
    {
        public CcrSpace()
        {}


        internal CcrSpace(CcrSpaceFluent fluent)
        {}


        public CcrsListenerFluent<TMessage> Listener<TMessage>()
        {
            return new CcrsListenerFluent<TMessage>();
        }


        public CcrsOneWayListener<TMessage> CreateListener<TMessage>(Action<TMessage> messageHandler)
        {
            return new CcrsOneWayListener<TMessage>(messageHandler);
        }

        public CcrsRequestResponseListener<TRequest, TResponse> CreateListener<TRequest, TResponse>(Func<TRequest, TResponse> requestHandler)
        {
            return new CcrsRequestResponseListener<TRequest, TResponse>();        
        }

        public CcrsRequestResponseListener<TRequest, TResponse> CreateListener<TRequest, TResponse>(Action<TRequest, ICcrsSimplexChannel<TResponse>> requestHandler)
        {
            return new CcrsRequestResponseListener<TRequest, TResponse>();
        }



        public CcrsPublisher<TBroadcastMessage> CreatePublisher<TBroadcastMessage>()
        {
            return new CcrsPublisher<TBroadcastMessage>();
        }


        public CcrsActor CreateActor(Func<CcrsActorContext, IEnumerator<ITask>> actorMethod)
        {
            return new CcrsActor();
        }


        public CcrsTryCatch TryCatch(Action<Exception> exceptionHandler)
        {
            return new CcrsTryCatch();
        }

        public CcrsTryCatch TryCatch(ICcrsSimplexChannel<Exception> exceptionSimplexChannel)
        {
            return new CcrsTryCatch();
        }


        #region Implementation of IDisposable
        public void Dispose()
        {}
        #endregion
    }
}
