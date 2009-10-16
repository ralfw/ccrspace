using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
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


        public static void HostPort<T>(this ICcrSpace space, Port<T> port, string name) { space.Resolve<CcrsHost>().HostPort(port, name); }
        public static void HostPort<TInput, TOutput>(this ICcrSpace space, PortSet<TInput, CcrsRequest<TInput, TOutput>> ports, string name) { space.Resolve<CcrsHost>().HostPort(ports, name); }
        public static void HostAsyncComponent<TAsyncContract>(this ICcrSpace space, TAsyncContract worker, string name)
            where TAsyncContract : IPort { space.HostAsyncComponent(worker, name); }


        public static Port<T> ConnectToPort<T>(this ICcrSpace space, string remoteAddress) { return space.Resolve<CcrsHost>().ConnectToPort<T>(remoteAddress); }
        public static PortSet<TInput, CcrsRequest<TInput, TOutput>> ConnectToPort<TInput, TOutput>(this ICcrSpace space, string remoteAddress) { return space.ConnectToPort<PortSet<TInput, CcrsRequest<TInput, TOutput>>>(remoteAddress); }
        public static TAsyncContract ConnectToAsyncComponent<TAsyncContract>(this ICcrSpace space, string remoteAddress)
            where TAsyncContract : IPort, new() { return space.ConnectToAsyncComponent<TAsyncContract>(remoteAddress); }
    }
}
