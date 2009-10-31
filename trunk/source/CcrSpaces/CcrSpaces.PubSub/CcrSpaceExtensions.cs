using CcrSpaces.Channels;
using CcrSpaces.Core;

namespace CcrSpaces.PubSub
{
    public static class CcrSpaceExtensions
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
