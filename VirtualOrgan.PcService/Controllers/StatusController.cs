using Microsoft.AspNetCore.Mvc;
using VirtualOrgan.PcService.ApiModel;

namespace VirtualOrgan.PcService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IPcService service;

        public StatusController(IPcService service)
        {
            this.service = service;
        }

        [HttpGet]
        public Status Get()
        {
            var pcStatus = service.GetStatus();
            return new Status
            {
                Running = pcStatus.IsHauptwerkRunning,
                AudioActive = pcStatus.IsHauptwerkAudioActive,
                MidiActive = pcStatus.IsHauptwerkMidiActive
            };
        }
    }
}
