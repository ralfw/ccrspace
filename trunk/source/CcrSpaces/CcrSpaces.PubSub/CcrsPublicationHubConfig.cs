using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.PubSub
{
    public class CcrsPublicationHubConfig
    {
        public DispatcherQueue TaskQueue;
        public CcrsHandlerModes HandlerMode = CcrsHandlerModes.Sequential;
    }
}
