using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtualOrgan.PcService.ApiModel;

namespace VirtualOrgan.PcService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IPcService pcService;

        public MainController(IPcService pcService)
        {
            this.pcService = pcService;
        }

        [HttpPost("shutdown")]
        public Task<RequestResult> Shutdown()
        {
            pcService.ShutdownPc();
            return Task.FromResult<RequestResult>(null);
        }

        [HttpPost("restart-hauptwerk")]
        public Task<RequestResult> RestartHauptwerk()
        {
            pcService.RestartHauptwerk();
            return Task.FromResult<RequestResult>(null);
        }

        [HttpPost("reset-midi-audio")]
        public Task<RequestResult> ResetMidiAndAudio()
        {
            pcService.ResetMidiAndAudio();
            return Task.FromResult<RequestResult>(null);
        }
    }
}