using VirtualOrgan.PcService.Midi;

namespace VirtualOrgan.PcService.Hauptwerk
{
    internal sealed class HauptwerkConfiguration
    {
        internal struct MidiNote
        {
            public int Note { get; set; }
            public int Channel { get; set; }

            public MidiNote(int note, int channel)
            {
                Note = note;
                Channel = channel;
            }

            public bool IsSame(NoteMessage m)
            {
                return Channel == m.Channel
                    && Note == m.Note;
            }
        }

        public string HauptwerkExePath { get; set; } = @"C:\Program Files\Hauptwerk Virtual Pipe Organ\Hauptwerk.exe";

        public MidiNote Play { get; set; } = Note(1, 6);
        public MidiNote Stop { get; set; } = Note(2, 6);
        public MidiNote Reset { get; set; } = Note(3, 6);
        public MidiNote OutAudioActive { get; set; } = Note(1, 6);
        public MidiNote OutMidiActive { get; set; } = Note(2, 6);

        private static MidiNote Note(int note, int channel)
        {
            return new MidiNote(note, channel);
        }
    }
}
