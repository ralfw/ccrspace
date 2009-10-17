using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.PubSub
{
    public class CcrsPublicationHubConfig<T>
    {
        public DispatcherQueue TaskQueue;
        public CcrsChannelHandlerModes HandlerMode = CcrsChannelHandlerModes.Sequential;
    }
}
