using System;
using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Core.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.PubSub
{
    [Serializable]
    public class CcrsSubscribe<T>
    {
        public string Key;
        public Port<T> Subscriber;

        public CcrsSubscribe(string key, Action<T> subscriptionHandler)
            : this(key, new CcrsChannelFactory().CreateChannel(new CcrsOneWayChannelConfig<T>
                                                                      {
                                                                          MessageHandler = subscriptionHandler
                                                                      })) {}
        public CcrsSubscribe(string key, Port<T> subscriber)
        {
            this.Key = key;
            this.Subscriber = subscriber;
        }
    }

    [Serializable]
    public class CcrsUnsubscribe
    {
        public string Key;
        public CcrsUnsubscribe(string key) { this.Key = key; }
    }



    public class CcrsSubscriptionManager<T> : PortSet<CcrsSubscribe<T>, CcrsUnsubscribe>
    {
        private readonly ReaderWriterLock rwl = new ReaderWriterLock();

        private readonly Dictionary<string, Port<T>> subscribers = new Dictionary<string, Port<T>>();


        public CcrsSubscriptionManager(){}

        internal CcrsSubscriptionManager(CcrsPublicationHubConfig config)
        {
            var chf = new CcrsChannelFactory();

            chf.ConfigureChannel(this.P0, new CcrsOneWayChannelConfig<CcrsSubscribe<T>>
                                                {
                                                    MessageHandler = this.ProcessSubscribe,
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = CcrsHandlerModes.Parallel
                                                });
            chf.ConfigureChannel(this.P1, new CcrsOneWayChannelConfig<CcrsUnsubscribe>
                                                {
                                                    MessageHandler = this.ProcessUnsubscribe,
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = CcrsHandlerModes.Parallel
                                                });
        }


        internal void ProcessSubscribe(CcrsSubscribe<T> subscription)
        {
            this.rwl.AcquireWriterLock(500);
            try
            {
                this.subscribers.Add(subscription.Key, subscription.Subscriber);
            }
            finally
            {
                this.rwl.ReleaseWriterLock();
            }
        }


        internal void ProcessUnsubscribe(CcrsUnsubscribe subscription)
        {
            this.rwl.AcquireWriterLock(500);
            try
            {
                if (this.subscribers.ContainsKey(subscription.Key))
                    this.subscribers.Remove(subscription.Key);
            }
            finally
            {
                this.rwl.ReleaseWriterLock();
            }
        }


        internal void ProcessPublish(T message)
        {
            this.rwl.AcquireReaderLock(500);
            try
            {
                foreach (Port<T> subscriber in this.subscribers.Values)
                    subscriber.Post(message);
            }
            finally
            {
                this.rwl.ReleaseReaderLock();
            }
        }
    }
}
