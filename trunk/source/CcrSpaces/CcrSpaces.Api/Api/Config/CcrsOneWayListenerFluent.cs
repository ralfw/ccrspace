using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    public class CcrsOneWayListenerFluent<TMessage>
    {
        private readonly CcrsOneWayListenerConfig<TMessage> cfg = new CcrsOneWayListenerConfig<TMessage>();


        public CcrsOneWayListenerFluent<TMessage> ProcessWith(Action<TMessage> messageHandler)
        {
            this.cfg.MessageHandler = messageHandler;
            return this;
        }


        public CcrsOneWayListenerFluent<TMessage> Sequentially()
        {
            this.cfg.ProcessSequentially = true;
            return this;
        }

        public CcrsOneWayListenerFluent<TMessage> Sequentially(bool processMessagesSequentially)
        {
            this.cfg.ProcessSequentially = processMessagesSequentially;
            return this;
        }


        public CcrsOneWayListener<TMessage> Create()
        {
            return new CcrsOneWayListener<TMessage>(cfg);
        }

        
        public static implicit operator CcrsOneWayListenerConfig<TMessage>(CcrsOneWayListenerFluent<TMessage> source)
        {
            return source.cfg;            
        }
    }
}
