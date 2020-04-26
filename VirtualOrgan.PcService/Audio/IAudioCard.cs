using System.Threading.Tasks;

namespace VirtualOrgan.PcService.Audio
{
    interface IAudioCard
    {
        Task<bool> IsActiveAsync();
    }
}
