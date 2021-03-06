using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualOrgan.PcService.Audio;
using VirtualOrgan.PcService.Hauptwerk;

namespace VirtualOrgan.PcService
{
    sealed class RestartHauptwerkTaskFactory : ITaskFactory
    {
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly IProcessHelper processHelper;
        private readonly IAudioCard audioCard;
        private readonly IOptions<RestartHauptwerkConfiguration> options;
        private readonly ILogger<RestartHauptwerkTask> logger;

        public RestartHauptwerkTaskFactory(
            IHauptwerkMidiInterface hauptwerk,
            IProcessHelper processHelper,
            IAudioCard audioCard,
            IOptions<RestartHauptwerkConfiguration> options,
            ILogger<RestartHauptwerkTask> logger)
        {
            this.hauptwerk = hauptwerk;
            this.processHelper = processHelper;
            this.audioCard = audioCard;
            this.options = options;
            this.logger = logger;
        }

        public Operation Operation => Operation.RestartHauptwerk;

        public ITaskWrapper Create() =>
            new RestartHauptwerkTask(
                hauptwerk,
                processHelper,
                audioCard,
                options,
                logger);
    }
}