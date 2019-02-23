using System;

namespace VirtualOrgan.PcService.Hauptwerk
{
    public interface IHauptwerkMidiInterface
    {
        void StartMidiPlayback();
        void StopMidiPlayback();
        void ResetAudioAndMidi();
        IObservable<HauptwerkStatus> HauptwerkStatuses { get; }
    }
}
