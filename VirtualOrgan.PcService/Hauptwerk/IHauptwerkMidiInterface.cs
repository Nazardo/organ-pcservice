using System;

namespace VirtualOrgan.PcService.Hauptwerk
{
    public interface IHauptwerkMidiInterface
    {
        void ClearStatus();
        void StartMidiPlayback();
        void StopMidiPlayback();
        void Quit();
        void ResetMidiAndAudio();
        IObservable<HauptwerkStatus> HauptwerkStatuses { get; }
    }
}
