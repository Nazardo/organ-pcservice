using System.Threading.Tasks;

namespace VirtualOrgan.PcService
{
    /// <summary>
    /// Interface exposed to the users of the current application
    /// </summary>
    public interface IPcService
    {
        Task<PcStatus> GetStatusAsync();
        void RestartHauptwerk();
        void ShutdownPc();
        void ResetMidiAndAudio();

        void PlayMidiFile(int id);
        void StopPlayback();
    }
}
