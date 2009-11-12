using System;
using CcrSpaces.Core.Services;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public interface ICcrSpace : ICcrsServiceRegistry
    {
        DispatcherQueue DefaultTaskQueue { get; }
    }
}
