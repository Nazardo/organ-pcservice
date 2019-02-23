namespace VirtualOrgan.PcService.Midi
{
    interface IMidiMessageSerializer
    {
        MidiMessage Deserialize(byte[] data);
        byte[] Serialize(MidiMessage message);
    }
}
