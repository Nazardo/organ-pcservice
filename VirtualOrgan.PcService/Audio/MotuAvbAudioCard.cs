using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VirtualOrgan.PcService.Audio
{
    sealed class MotuAvbAudioCard : IAudioCard
    {
        private readonly string apiTestUrl;
        private readonly ILogger<MotuAvbAudioCard> logger;

        public MotuAvbAudioCard(
            ILogger<MotuAvbAudioCard> logger,
            IOptions<MotuAvbConfiguration> options)
        {
            apiTestUrl = options.Value.ApiTestUrl;
            this.logger = logger;
        }

        public async Task<bool> IsActiveAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiTestUrl);
                    logger.LogDebug("Call to \"{0}\" returned {1} ({2})",
                        apiTestUrl, response.StatusCode, (int)response.StatusCode);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (HttpRequestException e)
            {
                logger.LogDebug(e.Message);
                return false;
            }
        }
    }
}
