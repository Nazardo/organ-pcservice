using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VirtualOrgan.PcService.Hauptwerk
{
    sealed class MotuAvbAudioCardHelper : IAudioCardHelper
    {
        private readonly string apiVersionUri;
        private static readonly string ApiVersion = "/apiversion";
        private readonly ILogger<MotuAvbAudioCardHelper> logger;

        public MotuAvbAudioCardHelper(
            ILogger<MotuAvbAudioCardHelper> logger,
            IOptions<MotuAvbConfiguration> options)
        {
            apiVersionUri = options.Value.LocalhostUsbEndpoint + ApiVersion;
            this.logger = logger;
        }

        public async Task<bool> IsAudioCardActive()
        {
            return await IsUsbInterfaceDetected();
        }

        private async Task<bool> IsUsbInterfaceDetected()
        {
            // Perform a test query to version api
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiVersionUri);
                logger.LogDebug("Call to \"{0}\" returned {1} ({1:d})", apiVersionUri, response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException e)
            {
                logger.LogDebug(e.Message);
                return false;
            }
        }
    }
}
