using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Config.Fluent
{
    public class CcrsOneWayChannelFluent<TMessage>
    {
        private readonly Dispatcher defaultDispatcher;

        private readonly CcrsOneWayChannelConfig<TMessage> cfg = new CcrsOneWayChannelConfig<TMessage>
                                                                    {
                                                                        ProcessSequentially = false         
                                                                    };


        public CcrsOneWayChannelFluent() : this(null, null)
        {}
        internal CcrsOneWayChannelFluent(Dispatcher defaultDispatcher, DispatcherQueue defaultTaskQueue)
        {
            this.defaultDispatcher = defaultDispatcher;
            this.cfg.TaskQueue = defaultTaskQueue ?? new DispatcherQueue();
        }



        public CcrsOneWayChannelFluent<TMessage> Process(Action<TMessage> messageHandler)
        {
            this.cfg.MessageHandler = messageHandler;
            return this;
        }


        public CcrsOneWayChannelFluent<TMessage> Sequentially()
        {
            this.cfg.ProcessSequentially = true;
            return this;
        }


        public CcrsOneWayChannelFluent<TMessage> WithOwnTaskQueue()
        {
            if (this.defaultDispatcher != null)
            {
                this.cfg.TaskQueue = new DispatcherQueue(string.Format("Dpq{0}.Dpq{1}",
                                                                       this.defaultDispatcher.Name,
                                                                       this.defaultDispatcher.DispatcherQueues.Count + 1),
                                                         this.defaultDispatcher);
            }
            return this;
        }


        public CcrsOneWayChannel<TMessage> Create()
        {
            return new CcrsOneWayChannel<TMessage>(cfg);
        }

        
        public static implicit operator CcrsOneWayChannelConfig<TMessage>(CcrsOneWayChannelFluent<TMessage> source)
        {
            return source.cfg;            
        }

        public static implicit operator CcrsOneWayChannel<TMessage>(CcrsOneWayChannelFluent<TMessage> source)
        {
            return source.Create();
        }
    }
}
