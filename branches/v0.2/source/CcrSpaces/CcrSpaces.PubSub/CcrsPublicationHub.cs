using System.Collections.Generic;
using System.Threading;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.PubSub
{
    public class CcrsPublicationHub<T> : Port<T>
    {
        private readonly CcrsSubscriptions<T> subscriptions;


        internal CcrsPublicationHub(CcrsPublicationHubConfig<T> config)
        {
            this.subscriptions = new CcrsSubscriptions<T>(config);

            new CcrsChannelFactory().ConfigureChannel<T>(
                        this,
                        new CcrsChannelConfig<T>{
                                                    MessageHandler = this.subscriptions.ProcessPublish,
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.HandlerMode
                                                });
        }


        public CcrsSubscriptions<T> Subscriptions
        {
            get { return this.subscriptions; }
        }
    }
}
