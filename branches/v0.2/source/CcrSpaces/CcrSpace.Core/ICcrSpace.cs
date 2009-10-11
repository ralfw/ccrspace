using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public interface ICcrSpace : IDisposable
    {
        DispatcherQueue DefaultTaskQueue { get; }
    }
}
