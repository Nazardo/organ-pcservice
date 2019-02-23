using System;

namespace VirtualOrgan.PcService.Midi
{
    interface IMidiInterface : IDisposable
    {
        void Send(MidiMessage message);
        IObservable<MidiMessage> Messages { get; }
    }
}
