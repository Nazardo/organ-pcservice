using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VirtualOrgan.PcService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HauptwerkController : ControllerBase
    {
        private readonly IPcService service;

        public HauptwerkController(IPcService service)
        {
            this.service = service;
        }

        // POST api/hauptwerk/start
        [HttpPost("start")]
        public async Task Start()
        {
            await service.StartHauptwerk();
        }

        // POST api/hauptwerk/play
        [HttpPost("play/{id}")]
        public void Play(int id)
        {
            service.PlayMidiFile(id);
        }

        // POST api/hauptwerk/stop
        [HttpPost("stop")]
        public void Stop()
        {
            service.StopPlayback();
        }

        // POST api/hauptwerk/reset
        [HttpPost("reset")]
        public void Reset()
        {
            service.ResetMidiAndAudio();
        }

        // POST api/hauptwerk/shutdown
        [HttpPost("shutdown")]
        public void Shutdown()
        {
            service.ShutdownPc();
        }
    }
}
