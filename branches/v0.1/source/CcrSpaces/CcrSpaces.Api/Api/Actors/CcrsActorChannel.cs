using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Api.Actors;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api.Actors
{
    public class CcrsActorChannel : ICcrsSimplexChannel<object>
    {
        private readonly Port<CcrsActorDialogMessage> channel;
        private readonly CcrsActorContext ctx;


        public CcrsActorChannel(CcrsActorBase objectActor) : this(objectActor.Run, new DispatcherQueue()) { }
        public CcrsActorChannel(Func<CcrsActorContext, IEnumerator<ITask>> methodActor) : this(methodActor, new DispatcherQueue()) {}

        public CcrsActorChannel(CcrsActorBase objectActor, DispatcherQueue taskQueue) : this(objectActor.Run, taskQueue) { }
        public CcrsActorChannel(Func<CcrsActorContext, IEnumerator<ITask>> methodActor, DispatcherQueue taskQueue)
        {
            this.channel = new Port<CcrsActorDialogMessage>();
            this.ctx = new CcrsActorContext(this, this.channel);

            Arbiter.Activate(
                taskQueue,
                new IterativeTask<CcrsActorContext>(
                        ctx, 
                        new IteratorHandler<CcrsActorContext>(methodActor))
                );
        }


        public void Post(object message)
        {
            this.channel.Post(new CcrsActorDialogMessage{Message=message});
        }

        public void Post(object message, CcrsActorChannel sender)
        {
            this.channel.Post(new CcrsActorDialogMessage { Message = message, Sender=sender });
        }


        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            this.Post(item);
        }

        public bool TryPostUnknownType(object item)
        {
            this.Post(item);
            return true;
        }

        #endregion
    }
}
