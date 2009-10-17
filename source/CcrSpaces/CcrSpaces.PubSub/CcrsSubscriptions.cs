﻿using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.PubSub
{
    public class CcrsSubscribe<T>
    {
        public string Key;
        public Port<T> Subscriber;

        public CcrsSubscribe(string key, Port<T> subscriber)
        {
            this.Key = key;
            this.Subscriber = subscriber;
        }
    }

    public class CcrsUnsubscribe
    {
        public string Key;
        public CcrsUnsubscribe(string key) { this.Key = key; }
    }



    public class CcrsSubscriptions<T> : PortSet<CcrsSubscribe<T>, CcrsUnsubscribe>
    {
        private readonly ReaderWriterLock rwl = new ReaderWriterLock();

        private readonly Dictionary<string, Port<T>> subscribers = new Dictionary<string, Port<T>>();


        internal CcrsSubscriptions(CcrsPublicationHubConfig<T> config)
        {
            var chf = new CcrsChannelFactory();

            chf.ConfigureChannel(this.P0, new CcrsChannelConfig<CcrsSubscribe<T>>
                                                {
                                                    MessageHandler = this.ProcessSubscribe,
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = CcrsChannelHandlerModes.Parallel
                                                });
            chf.ConfigureChannel(this.P1, new CcrsChannelConfig<CcrsUnsubscribe>
                                                {
                                                    MessageHandler = this.ProcessUnsubscribe,
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = CcrsChannelHandlerModes.Parallel
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
