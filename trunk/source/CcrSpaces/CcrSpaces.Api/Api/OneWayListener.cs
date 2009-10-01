using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsOneWayListener<TMessage> : ICcrsSimplexChannel<TMessage>
    {
        private readonly Port<TMessage> channel;


        public CcrsOneWayListener(Action<TMessage> messageHandler)
            : this(new CcrsOneWayListenerConfig<TMessage> { MessageHandler = messageHandler, TaskQueue = new DispatcherQueue(), ProcessSequentially = false })
        { }
        public CcrsOneWayListener(Action<TMessage> messageHandler, DispatcherQueue taskQueue)
            : this(new CcrsOneWayListenerConfig<TMessage> { MessageHandler = messageHandler, TaskQueue = taskQueue, ProcessSequentially = false })
        { }

        public CcrsOneWayListener(CcrsOneWayListenerConfig<TMessage> cfg)
        {
            this.channel = new Port<TMessage>();
            this.channel.RegisterHandler(cfg.MessageHandler, cfg.TaskQueue, cfg.ProcessSequentially);
        }


        public void Post(TMessage message)
        {
            this.channel.Post(message);
        }
    }
}
