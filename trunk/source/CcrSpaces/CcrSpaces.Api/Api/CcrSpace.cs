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
        private readonly Dispatcher defaultDispatcher;
        private readonly DispatcherQueue defaultDispatcherQueue;

        public CcrSpace()
        {
            this.defaultDispatcher = new Dispatcher();
            this.defaultDispatcherQueue = new DispatcherQueue("~default", this.defaultDispatcher);
        }


        public CcrsOneWayListener<TMessage> CreateListener<TMessage>(Action<TMessage> messageHandler)
        {
            var cfg = new CcrsOneWayListenerConfig<TMessage> {MessageHandler=messageHandler, TaskQueue=this.defaultDispatcherQueue};
            return new CcrsOneWayListener<TMessage>(cfg);
        }
        public CcrsRequestResponseListener<TRequest, TResponse> CreateListener<TRequest, TResponse>(Func<TRequest, TResponse> requestHandler)
        {
            throw new NotImplementedException();
        }
        public CcrsRequestResponseListener<TRequest, TResponse> CreateListener<TRequest, TResponse>(Action<TRequest, ICcrsSimplexChannel<TResponse>> requestHandler)
        {
            throw new NotImplementedException();
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
        {
            this.defaultDispatcher.Dispose();
        }
        #endregion
    }
}
