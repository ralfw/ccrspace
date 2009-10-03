using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    public class CcrsOneWayChannelFluent<TMessage>
    {
        private readonly CcrsOneWayChannelConfig<TMessage> cfg = new CcrsOneWayChannelConfig<TMessage>();


        public CcrsOneWayChannelFluent<TMessage> ProcessWith(Action<TMessage> messageHandler)
        {
            this.cfg.MessageHandler = messageHandler;
            return this;
        }


        public CcrsOneWayChannelFluent<TMessage> Sequentially()
        {
            this.cfg.ProcessSequentially = true;
            return this;
        }

        public CcrsOneWayChannelFluent<TMessage> Sequentially(bool processMessagesSequentially)
        {
            this.cfg.ProcessSequentially = processMessagesSequentially;
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
    }
}
