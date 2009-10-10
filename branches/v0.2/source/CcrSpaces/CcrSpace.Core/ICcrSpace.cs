using System;
using Microsoft.Ccr.Core;

namespace CcrSpace.Core
{
    public interface ICcrSpace : IDisposable
    {
        DispatcherQueue DefaultTaskQueue { get; }
    }
}
