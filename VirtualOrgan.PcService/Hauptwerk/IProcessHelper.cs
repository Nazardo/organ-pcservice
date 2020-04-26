namespace VirtualOrgan.PcService.Hauptwerk
{
    public interface IProcessHelper
    {
        bool IsRunning();
        void Start();
        void KillAll();
    }
}
