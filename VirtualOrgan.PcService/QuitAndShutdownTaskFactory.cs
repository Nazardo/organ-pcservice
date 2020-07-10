using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualOrgan.PcService.Hauptwerk;

namespace VirtualOrgan.PcService
{
    sealed class QuitAndShutdownTaskFactory : ITaskFactory
    {
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly IProcessHelper processHelper;
        private readonly IShutdownProvider shutdownHelper;
        private readonly IOptions<QuitAndShutdownConfiguration> options;
        private readonly ILogger<QuitAndShutdownTask> logger;

        public QuitAndShutdownTaskFactory(
            IHauptwerkMidiInterface hauptwerk,
            IProcessHelper processHelper,
            IShutdownProvider shutdownHelper,
            IOptions<QuitAndShutdownConfiguration> options,
            ILogger<QuitAndShutdownTask> logger)
        {
            this.hauptwerk = hauptwerk;
            this.processHelper = processHelper;
            this.shutdownHelper = shutdownHelper;
            this.options = options;
            this.logger = logger;
        }

        public Operation Operation => Operation.QuitAndShutdown;

        public ITaskWrapper Create() =>
            new QuitAndShutdownTask(
                hauptwerk,
                processHelper,
                shutdownHelper,
                options,
                logger);
    }
}