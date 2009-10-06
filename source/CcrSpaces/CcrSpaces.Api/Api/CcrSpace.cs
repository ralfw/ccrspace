using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Config;
using CcrSpaces.Api.Config.Fluent;
using CcrSpaces.Api.Flows;
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



        public CcrsOneWayChannelFluent<TMessage> CreateChannel<TMessage>()
        {
            return new CcrsOneWayChannelFluent<TMessage>(this.defaultDispatcher, this.defaultDispatcherQueue);
        }


        public ICcrsSimplexChannel<TMessage> CreateChannel<TMessage>(Action<TMessage> messageHandler) { return CreateChannel(messageHandler, false); }
        public ICcrsSimplexChannel<TMessage> CreateChannel<TMessage>(Action<TMessage> messageHandler, bool processSequentially)
        {
            var cfg = new CcrsOneWayChannelConfig<TMessage> {MessageHandler=messageHandler, TaskQueue=this.defaultDispatcherQueue, ProcessSequentially=processSequentially};
            return new CcrsOneWayChannel<TMessage>(cfg);
        }

        public ICcrsDuplexChannel<TRequest, TResponse> CreateChannel<TRequest, TResponse>(Func<TRequest, TResponse> messageHandler) { return CreateChannel(messageHandler, false); }
        public ICcrsDuplexChannel<TRequest, TResponse> CreateChannel<TRequest, TResponse>(Func<TRequest, TResponse> messageHandler, bool processSequentially)
        {
            var cfg = new CcrsRequestSingleResponseChannelConfig<TRequest, TResponse> {MessageHandler=messageHandler, TaskQueue=this.defaultDispatcherQueue, ProcessSequentially=processSequentially};
            return new CcrsRequestSingleResponseChannel<TRequest, TResponse>(cfg);
        }

        public ICcrsDuplexChannel<TRequest, TResponse> CreateChannel<TRequest, TResponse>(Action<TRequest, ICcrsSimplexChannel<TResponse>> messageHandler) { return CreateChannel(messageHandler, false); }
        public ICcrsDuplexChannel<TRequest, TResponse> CreateChannel<TRequest, TResponse>(Action<TRequest, ICcrsSimplexChannel<TResponse>> messageHandler, bool processSequentially)
        {
            var cfg = new CcrsRequestMultiResponseChannelConfig<TRequest, TResponse> { MessageHandler = messageHandler, TaskQueue = this.defaultDispatcherQueue, ProcessSequentially=processSequentially };
            return new CcrsRequestMultiResponseChannel<TRequest, TResponse>(cfg);
        }



        public CcrsPublisher<TBroadcastMessage> CreatePublisher<TBroadcastMessage>()
        {
            return new CcrsPublisher<TBroadcastMessage>(this.defaultDispatcherQueue);
        }


        public CcrsFlowFluent<TInput> CreateFlow<TInput>()
        {
            return new CcrsFlowFluent<TInput>();
        }
        
        public CcrsFlowFluent<TInput, TOutput> CreateFlow<TInput, TOutput>()
        {
            return new CcrsFlowFluent<TInput, TOutput>();
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
