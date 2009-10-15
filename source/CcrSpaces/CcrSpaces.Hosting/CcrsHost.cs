using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CcrSpaces.Channels;
using Microsoft.Ccr.Core;
using XcoAppSpaces.Core;

namespace CcrSpaces.Hosting
{
    public class CcrsHost : IDisposable
    {
        private readonly XcoAppSpace appSpace;

        public CcrsHost() { this.appSpace = new XcoAppSpace(); }
        public CcrsHost(string configString) { this.appSpace = new XcoAppSpace(configString); }


        public void Host<T>(Port<T> port, string name) { this.appSpace.RunWorker(port, name); }
        public void Host<TInput, TOutput>(PortSet<TInput, CcrsRequest<TInput, TOutput>> ports, string name) { this.appSpace.RunWorker(ports, name); }
        public void Host<T0, T1>(PortSet<T0, T1> ports, string name) { this.appSpace.RunWorker(ports, name); }
        public void Host<T0, T1, T2>(PortSet<T0, T1, T2> ports, string name) { this.appSpace.RunWorker(ports, name); }
        public void Host<TAsyncContract>(TAsyncContract worker, string name)
            where TAsyncContract : IPort { this.appSpace.RunWorker(worker, name); }


        public Port<T> ConnectToPort<T>(string remoteAddress) { return this.appSpace.ConnectWorker<Port<T>>(remoteAddress); }
        public PortSet<TInput, CcrsRequest<TInput, TOutput>> ConnectToPort<TInput, TOutput>(string remoteAddress) { return this.appSpace.ConnectWorker<PortSet<TInput, CcrsRequest<TInput, TOutput>>>(remoteAddress); }

        public PortSet<T0, T1> ConnectToPortSet<T0, T1>(string remoteAddress) { return this.appSpace.ConnectWorker<PortSet<T0, T1>>(remoteAddress); }
        public PortSet<T0, T1, T2> ConnectToPortSet<T0, T1, T2>(string remoteAddress) { return this.appSpace.ConnectWorker<PortSet<T0, T1, T2>>(remoteAddress); }
        public TAsyncContract ConnectToPortSet<TAsyncContract>(string remoteAddress) 
            where TAsyncContract : IPort, new() { return this.appSpace.ConnectWorker<TAsyncContract>(remoteAddress); }


        #region Implementation of IDisposable

        public void Dispose()
        {
            this.appSpace.Dispose();
        }

        #endregion
    }
}
