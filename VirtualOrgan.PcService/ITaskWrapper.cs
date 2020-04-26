using System;
using System.Threading.Tasks;

namespace VirtualOrgan.PcService
{
    /// <summary>
    /// Disposable wrapper of a task.
    /// Disposing the object cancel the running task.
    /// </summary>
    interface ITaskWrapper : IDisposable
    {
        Task Task { get; }
    }
}
