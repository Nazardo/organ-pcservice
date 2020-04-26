using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Options;
using VirtualOrgan.PcService.Midi;

namespace VirtualOrgan.PcService.Hauptwerk
{
    sealed class HauptwerkMidiInterface : IHauptwerkMidiInterface
    {
        private readonly HauptwerkConfiguration config;
        private readonly IMidiInterface midi;
        private HauptwerkStatus status;

        public IObservable<HauptwerkStatus> HauptwerkStatuses { get; }

        public HauptwerkMidiInterface(IOptions<HauptwerkConfiguration> options, IMidiInterface midi)
        {
            config = options.Value;
            this.midi = midi;
            HauptwerkStatuses = midi.Messages.Select(m => ProcessMessage(m));
        }

        private HauptwerkStatus ProcessMessage(MidiMessage m)
        {
            if (m is NoteMessage noteMessage)
            {
                if (config.OutAudioActive.IsSame(noteMessage))
                {
                    status.IsHauptwerkAudioActive = noteMessage.IsOn;
                }
                else if (config.OutMidiActive.IsSame(noteMessage))
                {
                    status.IsHauptwerkMidiActive = noteMessage.IsOn;
                }
            }
            return status;
        }

        public void ClearStatus()
        {
            status.IsHauptwerkAudioActive = false;
            status.IsHauptwerkMidiActive = false;
        }

        public void ResetMidiAndAudio()
        {
            SendMessageAsConfigured(config.Reset);
        }

        public void StartMidiPlayback()
        {
            SendMessageAsConfigured(config.Play);
        }

        public void StopMidiPlayback()
        {
            SendMessageAsConfigured(config.Stop);
        }

        public void Quit()
        {
            SendMessageAsConfigured(config.Quit);
        }

        private void SendMessageAsConfigured(HauptwerkConfiguration.MidiNote command)
        {
            var message = BuildMessageFromConfiguration(command);
            midi.Send(message);
        }

        private static MidiMessage BuildMessageFromConfiguration(HauptwerkConfiguration.MidiNote command)
        {
            return new NoteMessage((ushort)command.Channel, (ushort)command.Note, true);
        }
    }
}
