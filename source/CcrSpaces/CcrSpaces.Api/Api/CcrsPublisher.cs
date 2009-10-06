using System;
using System.Collections.Generic;
using CcrSpaces.Api.Config;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Api
{
    public class CcrsPublisher<TBroadcastMessage> : ICcrsSimplexChannel<TBroadcastMessage>
    {
        private readonly Dictionary<Action<TBroadcastMessage>, ICcrsSimplexChannel<TBroadcastMessage>> subscriptionHandlers;
        protected readonly List<ICcrsSimplexChannel<TBroadcastMessage>> subscribers;

        private readonly DispatcherQueue taskQueue;


        public CcrsPublisher() : this(new DispatcherQueue())
        {}

        internal CcrsPublisher(DispatcherQueue taskQueue)
        {
            this.subscriptionHandlers = new Dictionary<Action<TBroadcastMessage>, ICcrsSimplexChannel<TBroadcastMessage>>();
            this.subscribers = new List<ICcrsSimplexChannel<TBroadcastMessage>>();

            this.taskQueue = taskQueue;
        }


        #region Implementation of ICcrsSimplexChannel<TBroadcastMessage>

        public DispatcherQueue TaskQueue
        {
            get { return taskQueue; }
        }

        public void Post(TBroadcastMessage message)
        {
            lock (this.subscribers)
                foreach (ICcrsSimplexChannel<TBroadcastMessage> subscriber in this.subscribers)
                    subscriber.Post(message);
        }
        #endregion


        public void Subscribe(Action<TBroadcastMessage> subscriptionHandler)
        {
            lock (this.subscriptionHandlers)
            {
                if (this.subscriptionHandlers.ContainsKey(subscriptionHandler)) return;

                var cfg = new CcrsOneWayChannelConfig<TBroadcastMessage>
                              {
                                  MessageHandler = subscriptionHandler,
                                  TaskQueue = this.taskQueue,
                                  ProcessSequentially = true
                              };
                var ch = new CcrsOneWayChannel<TBroadcastMessage>(cfg);
                this.subscriptionHandlers.Add(subscriptionHandler, ch);

                this.Subscribe(ch);
            }
        }

        public void Subscribe(ICcrsSimplexChannel<TBroadcastMessage> subscriberChannel)
        {
            lock (this.subscribers)
                this.subscribers.Add(subscriberChannel);
        }


        public void Unsubscribe(Action<TBroadcastMessage> subscriptonHandler)
        {
            lock(this.subscriptionHandlers)
            {
                if (!this.subscriptionHandlers.ContainsKey(subscriptonHandler)) return;

                this.Unsubscribe(this.subscriptionHandlers[subscriptonHandler]);

                this.subscriptionHandlers.Remove(subscriptonHandler);
            }
        }

        public void Unsubscribe(ICcrsSimplexChannel<TBroadcastMessage> subscriberChannel)
        {
            lock(this.subscribers)
                this.subscribers.Remove(subscriberChannel);
        }


        #region Implementation of IPort

        public void PostUnknownType(object item)
        {
            this.Post((TBroadcastMessage)item);
        }

        public bool TryPostUnknownType(object item)
        {
            if (!(item is TBroadcastMessage)) return false;

            this.PostUnknownType(item);
            return true;
        }

        #endregion
    }
}
