using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Core.Services
{
    public interface ICcrsService
    {}

    public interface ICcrsResourcefulService : ICcrsService, IDisposable
    {}
}
