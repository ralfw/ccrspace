using CcrSpaces.Core.Channels;
using CcrSpaces.Core.Core;
using CcrSpaces.Core.PubSub;

namespace CcrSpaces.Core
{
    public static class CcrSpaceExtensionsForPubSub
    {
        public static CcrsPublicationHub<T> CreatePublicationHub<T>(this ICcrSpace space) 
        { return CreatePublicationHub<T>(space, CcrsHandlerModes.Sequential); }

        public static CcrsPublicationHub<T> CreatePublicationHub<T>(this ICcrSpace space, CcrsHandlerModes handlerMode)
        {
            return new CcrsPublicationHub<T>(new CcrsPublicationHubConfig
                                                 {
                                                     TaskQueue = space.DefaultTaskQueue,
                                                     HandlerMode = handlerMode
                                                 });
        }
    }
}
