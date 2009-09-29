using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    public class CcrsListenerFluent<TMessage>
    {
        private readonly CcrsListenerConfig<TMessage> cfg = new CcrsListenerConfig<TMessage>();


        public CcrsListenerFluent<TMessage> WithName(string name)
        {
            this.cfg.Name = name;
            return this;
        }

        public CcrsListenerFluent<TMessage> ProcessWith(Action<TMessage> messageHandler)
        {
            this.cfg.MessageHandler = messageHandler;
            return this;
        }

        public CcrsListenerFluent<TMessage> Sequentially(bool processMessagesSequentially)
        {
            this.cfg.ProcessSequentially = processMessagesSequentially;
            return this;
        }


        public CcrsListenerFluent<TMessage> InCurrentSyncContext(bool processMessagesInCurrentSyncContext)
        {
            throw new NotImplementedException();
        }


        public CcrsListenerFluent<TMessage> ScheduledByDispatcherQueue()
        {
            throw new NotImplementedException();
        }


        public CcrsOneWayListener<TMessage> Create()
        {
            return new CcrsOneWayListener<TMessage>(cfg);
        }
    }
}
