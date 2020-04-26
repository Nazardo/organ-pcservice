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
            Task = new Task(() =>
            {
                try
                {
                    Execute(token);
                }
                catch (OperationCanceledException)
                {
                    // swallow cancellation
                }
            }, token);
        }

        public Task Task { get; }

        protected abstract void Execute(CancellationToken cancellationToken);

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
