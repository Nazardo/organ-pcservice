using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualOrgan.PcService.Hauptwerk;

namespace VirtualOrgan.PcService
{
    sealed class QuitAndShutdownTask : TaskWrapperBase
    {
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly IProcessHelper processHelper;
        private readonly IShutdownProvider shutdownHelper;
        private readonly ILogger<QuitAndShutdownTask> logger;
        private readonly QuitAndShutdownConfiguration options;

        public QuitAndShutdownTask(
            IHauptwerkMidiInterface hauptwerk,
            IProcessHelper processHelper,
            IShutdownProvider shutdownHelper,
            IOptions<QuitAndShutdownConfiguration> options,
            ILogger<QuitAndShutdownTask> logger)
        {
            this.options = options.Value;
            this.hauptwerk = hauptwerk;
            this.processHelper = processHelper;
            this.shutdownHelper = shutdownHelper;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            int numberOfTriesWithMidi = options.NumberOfSoftTries;
            logger.LogInformation("Quit Hauptwerk (if running)");
            while (processHelper.IsRunning())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (numberOfTriesWithMidi > 0)
                {
                    numberOfTriesWithMidi--;
                    logger.LogDebug("Quit Hauptwerk via interface");
                    hauptwerk.Quit();
                    await Task.Delay(options.DelayAfterSoftQuitMs, cancellationToken);
                }
                else
                {
                    logger.LogDebug("Quit Hauptwerk with Kill");
                    processHelper.KillAll();
                    await Task.Delay(options.DelayAfterKillMs, cancellationToken);
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            logger.LogInformation("Request computer shutdown");
            shutdownHelper.Shutdown();
        }
    }

    sealed class QuitAndShutdownConfiguration
    {
        public int NumberOfSoftTries { get; set; } = 3;
        public int DelayAfterSoftQuitMs { get; set; } = 5000;
        public int DelayAfterKillMs { get; set; } = 500;
    }
}
