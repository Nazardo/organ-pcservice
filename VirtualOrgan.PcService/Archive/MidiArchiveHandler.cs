using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace VirtualOrgan.PcService.Archive
{
    internal sealed class MidiArchiveHandler : IMidiArchiveHandler
    {
        private readonly string activePath;
        private readonly Dictionary<int, string> paths;

        public MidiArchiveHandler(IOptions<MidiArchiveConfiguration> options)
        {
            activePath = options.Value.Destination;
            paths = new Dictionary<int, string>(
                options.Value.Files.Select((path, index) => KeyValuePair.Create(index, path)));
        }

        public void SetActive(int id)
        {
            if (paths.TryGetValue(id, out string path))
            {
                System.IO.File.Copy(path, activePath, true);
            }
        }
    }
}
