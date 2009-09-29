using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    public class CcrsListenerConfig<TMessage>
    {
        public CcrsListenerConfig<TMessage> WithName(string name)
        {
            return this;
        }

        public CcrsListenerConfig<TMessage> ProcessWith(Action<TMessage> messageHandler)
        {
            return this;
        }

        public CcrsListenerConfig<TMessage> Sequentially(bool processMessagesSequentially)
        {
            return this;
        }

        public CcrsListenerConfig<TMessage> InCurrentSyncContext(bool processMessagesInCurrentSyncContext)
        {
            return this;
        }


        public CcrsListenerConfig<TMessage> ScheduledByDispatcherQueue()
        {
            return this;
        }


        public CcrsOneWayListener<TMessage> Create()
        {
            return null;
        }
    }
}
