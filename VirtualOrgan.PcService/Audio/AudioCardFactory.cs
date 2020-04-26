using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VirtualOrgan.PcService.Audio
{
    sealed class AudioCardFactory : IFactory<IAudioCard>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string implementation;

        public AudioCardFactory(
            IServiceProvider serviceProvider,
            IOptions<FactoriesConfiguration> options)
        {
            this.serviceProvider = serviceProvider;
            implementation = options.Value.AudioCard;
        }

        public IAudioCard Create()
        {
            if (implementation.Equals("motu", StringComparison.InvariantCultureIgnoreCase))
            {
                return serviceProvider.GetRequiredService<MotuAvbAudioCard>();
            }
            return new AlwaysOnAudioCard();
        }
    }

    sealed class AlwaysOnAudioCard : IAudioCard
    {
        public Task<bool> IsActiveAsync() => Task.FromResult(true);
    }
}