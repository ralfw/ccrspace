using CcrSpaces.Core;
using CcrSpaces.Core.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public class CcrsPublicationHubConfig
    {
        public DispatcherQueue TaskQueue;
        public CcrsHandlerModes HandlerMode = CcrsHandlerModes.Sequential;
    }
}
