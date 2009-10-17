using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using CcrSpaces.Core;

namespace CcrSpaces.PubSub
{
    public static class CcrSpaceExtensions
    {
        public static CcrsPublicationHub<T> CreatePublicationHub<T>(this ICcrSpace space) 
        { return CreatePublicationHub<T>(space, CcrsChannelHandlerModes.Sequential); }

        public static CcrsPublicationHub<T> CreatePublicationHub<T>(this ICcrSpace space, CcrsChannelHandlerModes handlerMode)
        {
            return new CcrsPublicationHub<T>(new CcrsPublicationHubConfig<T>
                                                 {
                                                     TaskQueue = space.DefaultTaskQueue,
                                                     HandlerMode = handlerMode
                                                 });
        }
    }
}
