using System;
using Microsoft.Ccr.Core;
using Microsoft.Ccr.Core.Arbiters;

namespace CcrSpaces.Flows.Infrastructure
{
    partial class IPortReceiveExtensions
    {
        internal class ConsumingReceiverTask : ReceiverTask
        {
            private readonly Action<object> elementHandler;

            public ConsumingReceiverTask(Action<object> elementHandler)
            {
                base.State = ReceiverTaskState.Persistent;
                this.elementHandler = elementHandler;
            }

            public override void Cleanup(ITask task)
            {
                // nothing to do
            }

            public override bool Evaluate(IPortElement messageNode, ref ITask deferredTask)
            {
                deferredTask = null; // don´t try to queue the element
                this.elementHandler(messageNode.Item);
                return true;
            }

            public override void Consume(IPortElement item)
            { }
        }
    }
}
