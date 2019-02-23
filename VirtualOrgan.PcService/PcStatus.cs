namespace VirtualOrgan.PcService
{
    public struct PcStatus
    {
        public bool IsHauptwerkRunning { get; set; }
        public bool IsHauptwerkAudioActive { get; set; }
        public bool IsHauptwerkMidiActive { get; set; }
    }
}