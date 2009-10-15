using CcrSpaces.Core.Services;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public class CcrSpace : ICcrSpace, ICcrsServiceRegistry
    {
        protected readonly Dispatcher defaultDispatcher;
        protected readonly DispatcherQueue defaultTaskQueue;

        protected readonly ICcrsServiceRegistry serviceRegistry;


        public CcrSpace() : this(new Dispatcher()) {}
        public CcrSpace(Dispatcher defaultDispatcher)
        {
            this.defaultDispatcher = defaultDispatcher;
            this.defaultTaskQueue = new DispatcherQueue("DefaultTaskQueue", this.defaultDispatcher);

            this.serviceRegistry = new ServiceRegistry();
        }


        #region Implementation of ICcrSpace

        public DispatcherQueue DefaultTaskQueue
        {
            get { return this.defaultTaskQueue; }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.defaultDispatcher.Dispose();
        }

        #endregion


        #region ICcrsServiceRegistry Members

        void ICcrsServiceRegistry.Register(ICcrsService service)
        {
            this.serviceRegistry.Register(service);
        }

        void ICcrsServiceRegistry.Register(ICcrsService service, string name)
        {
            this.serviceRegistry.Register(service, name);
        }

        TService ICcrsServiceRegistry.Resolve<TService>()
        {
            return this.serviceRegistry.Resolve<TService>();
        }

        TService ICcrsServiceRegistry.Resolve<TService>(string name)
        {
            return this.serviceRegistry.Resolve<TService>(name);
        }

        System.Collections.Generic.IEnumerable<TService> ICcrsServiceRegistry.GetAll<TService>()
        {
            return this.serviceRegistry.GetAll<TService>();
        }

        #endregion
    }
}
