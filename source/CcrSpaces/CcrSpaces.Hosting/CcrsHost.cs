using CcrSpaces.Channels;
using CcrSpaces.Core.Services;
using Microsoft.Ccr.Core;
using XcoAppSpaces.Core;

namespace CcrSpaces.Hosting
{
    internal class CcrsHost : ICcrsResourcefulService
    {
        private readonly XcoAppSpace appSpace;

        public CcrsHost() { this.appSpace = new XcoAppSpace(); }
        public CcrsHost(string configString) { this.appSpace = new XcoAppSpace(configString); }


        public void HostPort<T>(Port<T> port, string name) { this.appSpace.RunWorker(port, name); }
        public void HostPort<TInput, TOutput>(PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType> ports, string name) { this.appSpace.RunWorker(ports, name); }
        public void HostAsyncComponent<TAsyncContract>(TAsyncContract worker, string name)
            where TAsyncContract : IPort { this.appSpace.RunWorker(worker, name); }


        public Port<T> ConnectToPort<T>(string remoteAddress) { return this.appSpace.ConnectWorker<Port<T>>(remoteAddress); }
        public PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType> ConnectToPort<TInput, TOutput>(string remoteAddress) { return this.appSpace.ConnectWorker<PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType>>(remoteAddress); }
        public TAsyncContract ConnectToAsyncComponent<TAsyncContract>(string remoteAddress) 
            where TAsyncContract : IPort, new() { return this.appSpace.ConnectWorker<TAsyncContract>(remoteAddress); }


        #region Implementation of IDisposable

        public void Dispose()
        {
            this.appSpace.Dispose();
        }

        #endregion
    }
}
