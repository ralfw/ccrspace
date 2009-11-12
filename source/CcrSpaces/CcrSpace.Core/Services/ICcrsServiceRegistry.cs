using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Core.Core.Services
{
    public interface ICcrsServiceRegistry : IDisposable
    {
        void Register(ICcrsService service);
        void Register(ICcrsService service, string name);

        TService Resolve<TService>() where TService : ICcrsService;
        TService Resolve<TService>(string name) where TService : ICcrsService;

        IEnumerable<TService> GetAll<TService>() where TService : ICcrsService;
    }
}
