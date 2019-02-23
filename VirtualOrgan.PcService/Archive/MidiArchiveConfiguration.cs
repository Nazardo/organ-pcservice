using System.Collections.Generic;

namespace VirtualOrgan.PcService.Archive
{
    internal sealed class MidiArchiveConfiguration
    {
        public List<string> Files { get; set; } = new List<string>();
        public string Destination { get; set; } = @"C:\MidiArchive\Hauptwerk.mid";
    }
}
