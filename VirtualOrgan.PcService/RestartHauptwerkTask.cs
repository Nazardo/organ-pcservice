using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualOrgan.PcService.Hauptwerk;

namespace VirtualOrgan.PcService
{
    sealed class RestartHauptwerkTask : TaskWrapperBase
    {
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly IProcessHelper processHelper;
        private readonly RestartHauptwerkConfiguration options;
        private readonly ILogger<RestartHauptwerkTask> logger;

        public RestartHauptwerkTask(
            IHauptwerkMidiInterface hauptwerk,
            IProcessHelper processHelper,
            IOptions<RestartHauptwerkConfiguration> options,
            ILogger<RestartHauptwerkTask> logger)
        {
            this.hauptwerk = hauptwerk;
            this.processHelper = processHelper;
            this.options = options.Value;
            this.logger = logger;
        }

        protected override void Execute(CancellationToken cancellationToken)
        {
            bool started = false;
            while (!started)
            {
                logger.LogInformation("Quitting Hauptwerk (if running)");
                int numberOfTriesWithMidi = options.NumberOfSoftTries;
                while (processHelper.IsRunning())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (numberOfTriesWithMidi > 0)
                    {
                        numberOfTriesWithMidi--;
                        hauptwerk.Quit();
                        Task.Delay(options.DelayAfterSoftQuitMs, cancellationToken);
                    }
                    else
                    {
                        processHelper.KillAll();
                        Task.Delay(options.DelayAfterKillMs, cancellationToken);
                    }
                }
                try
                {
                    hauptwerk.ClearStatus();
                    cancellationToken.ThrowIfCancellationRequested();
                    Task.Delay(options.WaitBeforeRestartMs, cancellationToken);
                    var completionTask = hauptwerk.HauptwerkStatuses
                        .Timeout(TimeSpan.FromMilliseconds(options.OrganLoadingTimeoutMs))
                        .TakeUntil(status => status.IsHauptwerkAudioActive && status.IsHauptwerkMidiActive)
                        .ToTask();
                    logger.LogInformation("Starting Hauptwerk");
                    processHelper.Start();
                    completionTask.Wait(cancellationToken);
                    started = true;
                    logger.LogInformation("Hauptwerk started");
                }
                catch (OperationCanceledException)
                {
                    logger.LogDebug("Start Hauptwerk cancelled");
                    throw;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error starting Hauptwerk");
                }
            }
        }
    }

    sealed class RestartHauptwerkConfiguration
    {
        public int NumberOfSoftTries { get; set; } = 3;
        public int DelayAfterSoftQuitMs { get; set; } = 5000;
        public int DelayAfterKillMs { get; set; } = 500;
        public int WaitBeforeRestartMs { get; set; } = 1000;
        public int DelayAfterStartProcessMs { get; set; } = 3000;
        public int OrganLoadingTimeoutMs { get; set; } = 20000;
    }
}
