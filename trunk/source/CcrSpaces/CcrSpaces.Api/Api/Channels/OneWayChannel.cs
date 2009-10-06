using System;
using CcrSpaces.Api.Config;
using CcrSpaces.Infrastructure;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsOneWayChannel<TMessage> : ICcrsSimplexChannel<TMessage>
    {
        private readonly Port<TMessage> channel;


        public CcrsOneWayChannel(Action<TMessage> messageHandler) : this(messageHandler, false) {}
        public CcrsOneWayChannel(Action<TMessage> messageHandler, bool processSequentially)
            : this(new CcrsOneWayChannelConfig<TMessage> { MessageHandler = messageHandler, TaskQueue = new DispatcherQueue(), ProcessSequentially = processSequentially })
        { }

        public CcrsOneWayChannel(CcrsOneWayChannelConfig<TMessage> cfg)
        {
            this.channel = new Port<TMessage>();
            this.channel.RegisterHandler(cfg.MessageHandler, cfg.TaskQueue, cfg.ProcessSequentially, cfg.ProcessInCurrentSyncContext);
        }


        public void Post(TMessage message)
        {
            this.channel.Post(message);
        }


        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            this.channel.PostUnknownType(item);
        }

        public bool TryPostUnknownType(object item)
        {
            return this.channel.TryPostUnknownType(item);
        }

        #endregion
    }
}
