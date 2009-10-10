using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Actors
{
    public class CcrsActorContext
    {
        private readonly CcrsActorChannel self;
        private readonly Port<CcrsActorDialogMessage> inputChannel;
        private readonly Task<CcrsActorDialogMessage> copyToReceivedMessage;

        private CcrsActorDialogMessage receivedMessage;


        internal CcrsActorContext(CcrsActorChannel self, Port<CcrsActorDialogMessage> inputChannel)
        {
            this.self = self;
            this.inputChannel = inputChannel;
            this.copyToReceivedMessage = new Task<CcrsActorDialogMessage>(am => this.receivedMessage = am);
        }


        public ITask Receive<T>() { return CreateReceiverForTypedValue(typeof(T)); }
        public ITask Receive<T0, T1>() { return CreateReceiverForTypedValue(typeof(T0), typeof(T1)); }
        public ITask Receive<T0, T1, T2>() { return CreateReceiverForTypedValue(typeof(T0), typeof(T1), typeof(T2)); }

        private ITask CreateReceiverForTypedValue(params Type[] expectedTypes)
        {
            return new Receiver<CcrsActorDialogMessage>(
                false,
                this.inputChannel,
                am => expectedTypes.Contains(am.Message.GetType()),
                this.copyToReceivedMessage);
        }


        public object ReceivedValue
        {
            get
            {
                return this.receivedMessage.Message;
            }
        }


        public CcrsActorChannel Self
        {
            get { return this.self;  }
        }


        public void Reply(object message)
        {
            if (this.receivedMessage.Sender == null) throw new InvalidOperationException("Last message received has no sender to reply to!");

            this.receivedMessage.Sender.Post(message, self);
        }
    }
}
