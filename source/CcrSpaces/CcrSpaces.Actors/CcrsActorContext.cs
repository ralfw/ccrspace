using System;
using System.Linq;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Actors
{
    public class CcrsActorContext
    {
        private readonly IPort self;
        private readonly Port<ActorDialogMessage> inputChannel;

        private ActorDialogMessage receivedMessage;


        internal CcrsActorContext(IPort self, Port<ActorDialogMessage> inputChannel)
        {
            this.self = self;
            this.inputChannel = inputChannel;
        }


        public ITask Receive<T>() { return CreateReceiverForTypedValue(typeof(T)); }
        public ITask Receive<T0, T1>() { return CreateReceiverForTypedValue(typeof(T0), typeof(T1)); }
        public ITask Receive<T0, T1, T2>() { return CreateReceiverForTypedValue(typeof(T0), typeof(T1), typeof(T2)); }

        private ITask CreateReceiverForTypedValue(params Type[] expectedTypes)
        {
            return new Receiver<ActorDialogMessage>(
                false,
                this.inputChannel,
                am => expectedTypes.Contains(am.Message.GetType()),
                new Task<ActorDialogMessage>(am => this.receivedMessage = am));
        }


        public object ReceivedValue
        { get { return this.receivedMessage.Message; } }


        public IPort Self
        { get { return this.self; } }



        public void Ask(IPort dialogPartner, object message)
        {
            dialogPartner.PostUnknownType(new ActorDialogMessage{Message=message, Sender=this.self});
        }


        public void Reply(object message)
        {
            if (this.receivedMessage.Sender == null) throw new InvalidOperationException("Last message received has no sender to reply to!");

            this.receivedMessage.Sender.PostUnknownType(new ActorDialogMessage{Message=message, Sender=self});
        }


        public CcrsPulsator PulsePeriodically(int pulsationPeriodMsec)
        {
            return new CcrsPulsator(self, pulsationPeriodMsec);
        }
    }
}
