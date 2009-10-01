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
        public CcrsRequestResponseListenerBase<TRequest, TResponse> CreateListener<TRequest, TResponse>(Func<TRequest, TResponse> messageHandler)
        {
            var cfg = new CcrsRequestSingleResponseListenerConfig<TRequest, TResponse> {MessageHandler=messageHandler, TaskQueue=this.defaultDispatcherQueue};
            return new CcrsRequestSingleResponseListener<TRequest, TResponse>(cfg);
        }
        public CcrsRequestResponseListenerBase<TRequest, TResponse> CreateListener<TRequest, TResponse>(Action<TRequest, ICcrsSimplexChannel<TResponse>> messageHandler)
        {
            var cfg = new CcrsRequestMultiResponseListenerConfig<TRequest, TResponse> { MessageHandler = messageHandler, TaskQueue = this.defaultDispatcherQueue };
            return new CcrsRequestMultiResponseListener<TRequest, TResponse>(cfg);
        }



        public CcrsPublisher<TBroadcastMessage> CreatePublisher<TBroadcastMessage>()
        {
            return new CcrsPublisher<TBroadcastMessage>();
        }


        public CcrsActor CreateActor(Func<CcrsActorContext, IEnumerator<ITask>> actorMethod)
        {
            return new CcrsActor();
        }


        public CcrsTry Try(Action tryThis)
        {
            return new CcrsTry(tryThis);
        }


        #region Implementation of IDisposable
        public void Dispose()
        {
            this.defaultDispatcher.Dispose();
        }
        #endregion
    }
}
