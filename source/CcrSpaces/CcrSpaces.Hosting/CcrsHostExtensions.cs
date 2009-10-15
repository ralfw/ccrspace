using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Core;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Hosting
{
    public static class CcrsHostExtensions
    {
        public static ICcrSpace ConfigureAsHost(this ICcrSpace space, string configurationString)
        {
            space.Register(new CcrsHost(configurationString));
            return space;
        }


        public static void Host<T>(this ICcrSpace space, Port<T> port, string name) { space.Resolve<CcrsHost>().Host(port, name); }
        
        public static Port<T> ConnectToPort<T>(this ICcrSpace space, string remoteAddress) { return space.Resolve<CcrsHost>().ConnectToPort<T>(remoteAddress); }
    }
}
