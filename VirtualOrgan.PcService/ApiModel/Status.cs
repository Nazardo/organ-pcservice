namespace VirtualOrgan.PcService.ApiModel
{
    public sealed class Status
    {
        public bool Running { get; set; }
        public bool AudioActive { get; set; }
        public bool MidiActive { get; set; }
    }
}
