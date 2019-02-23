namespace VirtualOrgan.PcService.Archive
{
    public struct MidiFile
    {
        public int Id { get; set; }
        public string Path { get; set; }

        public MidiFile(int id, string path)
        {
            Id = id;
            Path = path;
        }
    }
}
