using System;
using System.Collections.Generic;
using CcrSpaces.Core.Channels.Extensions;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Actors
{
    public class CcrsActor : Port<object>
    {
        private readonly Port<ActorDialogMessage> dialogPort;
        private readonly CcrsActorContext ctx;


        public CcrsActor(CcrsActorBase actor) : this(actor.Run){}
        public CcrsActor(Func<CcrsActorContext, IEnumerator<ITask>> actorMethod) : this(new CcrsActorConfig{ MessageHandler = actorMethod }) {}
        public CcrsActor(CcrsActorConfig config)
        {
            this.RegisterGenericSyncReceiver(this.TransformIncomingToDialogMessage);

            this.dialogPort = new Port<ActorDialogMessage>();
            this.ctx = new CcrsActorContext(this, this.dialogPort);

            Arbiter.Activate(
                config.TaskQueue ?? new DispatcherQueue(),
                new IterativeTask<CcrsActorContext>(
                        ctx,
                        new IteratorHandler<CcrsActorContext>(config.MessageHandler))
                );
        }


        private void TransformIncomingToDialogMessage(object message)
        {
            if (message is ActorDialogMessage)
                this.dialogPort.Post(message as ActorDialogMessage);
            else
                this.dialogPort.Post(new ActorDialogMessage {Message = message});
        }
    }
}
