using System.Threading.Tasks;

namespace VirtualOrgan.PcService
{
    /// <summary>
    /// Interface exposed to the users of the current application
    /// </summary>
    public interface IPcService
    {
        PcStatus GetStatus();
        Task StartHauptwerk();
        void PlayMidiFile(int id);
        void StopPlayback();
        void ResetMidiAndAudio();
        void ShutdownPc();
    }
}
