using System;
using System.Collections.Generic;
namespace CcrSpaces.Core.Core.Services
{
    internal class ServiceRegistry : ICcrsServiceRegistry
    {
        private readonly List<ICcrsService> services = new List<ICcrsService>();
        private readonly Dictionary<string, ICcrsService> serviceIndex = new Dictionary<string, ICcrsService>();


        #region Implementation of ICcrsServiceRegistry

        public void Register(ICcrsService service)
        {
            this.services.Add(service);
        }

        public void Register(ICcrsService service, string name)
        {
            this.serviceIndex.Add(name, service);
            Register(service);
        }


        public TService Resolve<TService>() where TService : ICcrsService
        {
            var service = this.services.Find(s => s is TService);
            if (service == null) throw new IndexOutOfRangeException(string.Format("No service of type {0} registered!", typeof(TService).Name));

            return (TService)service;
        }

        public TService Resolve<TService>(string name) where TService : ICcrsService
        {
            return (TService)this.serviceIndex[name];
        }


        public IEnumerable<TService> GetAll<TService>() where TService : ICcrsService
        {
            return this.services.FindAll(s => s is TService).ConvertAll(s => (TService)s);
        }

        #endregion


        #region Implementation of IDisposable

        public void Dispose()
        {
            foreach (IDisposable resourcefulService in this.services.FindAll(s => s is IDisposable).ConvertAll(s => (IDisposable)s))
                resourcefulService.Dispose();
        }

        #endregion
    }
}
