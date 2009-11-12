using System;
using CcrSpaces.Core.Core.Services;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core.Core
{
    public interface ICcrSpace : ICcrsServiceRegistry
    {
        DispatcherQueue DefaultTaskQueue { get; }
    }
}
