using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.PubSub
{
    public class CcrsPublicationHub<T> : Port<T>
    {
        private readonly CcrsSubscriptionManager<T> subscriptions;


        internal CcrsPublicationHub(CcrsPublicationHubConfig config)
        {
            this.subscriptions = new CcrsSubscriptionManager<T>(config);

            new CcrsChannelFactory().ConfigureChannel(
                        this,
                        new CcrsOneWayChannelConfig<T>{
                                                    MessageHandler = this.subscriptions.ProcessPublish,
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.HandlerMode
                                                });
        }


        public CcrsSubscriptionManager<T> Subscriptions
        {
            get { return this.subscriptions; }
        }
    }
}
