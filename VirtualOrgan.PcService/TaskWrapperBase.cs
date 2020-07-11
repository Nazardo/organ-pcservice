using System;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualOrgan.PcService
{
    abstract class TaskWrapperBase : ITaskWrapper
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private Task runningTask;

        protected TaskWrapperBase()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            runningTask = Task.Run(async () =>
            {
                try
                {
                    await ExecuteAsync(cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // swallow cancellation
                }
            });
        }

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
            if (disposing)
            {
                cancellationTokenSource.Cancel();
                if (runningTask != null)
                {
                    runningTask.Wait();
                    runningTask = null;
                }
                cancellationTokenSource.Dispose();
            }
        }
    }
}
