using System;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualOrgan.PcService
{
    abstract class TaskWrapperBase : ITaskWrapper
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        protected TaskWrapperBase()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            Task = ExecuteAsync(token);
        }

        public Task Task { get; }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

        ~TaskWrapperBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}
