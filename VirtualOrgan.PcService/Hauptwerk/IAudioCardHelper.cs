using System.Threading.Tasks;

namespace VirtualOrgan.PcService.Hauptwerk
{
    interface IAudioCardHelper
    {
        Task<bool> IsAudioCardActive();
    }
}
