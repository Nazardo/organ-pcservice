using System;
using System.Threading.Tasks;

namespace VirtualOrgan.PcService
{
    /// <summary>
    /// Disposable wrapper of a task.
    /// Disposing the object cancel the task if running.
    /// </summary>
    interface ITaskWrapper : IDisposable
    {
        void Start();
    }
}
