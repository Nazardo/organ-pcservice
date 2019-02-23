using System.Linq;

namespace VirtualOrgan.PcService.Midi.Impl
{
    internal sealed class MessageSerializer : IMidiMessageSerializer
    {
        public MidiMessage Deserialize(byte[] data)
        {
            int command = data[0] >> 4;
            switch (command)
            {
                case 0x08:
                    {
                        ushort channel = (ushort)((data[0] & 0x0F) + 1);
                        return new NoteMessage(channel, data[1], false)
                        {
                            Velocity = data[2]
                        };
                    }
                case 0x09:
                    {
                        ushort channel = (ushort)((data[0] & 0x0F) + 1);
                        return new NoteMessage(channel, data[1], true)
                        {
                            Velocity = data[2]
                        };
                    }
                case 0x0B:
                    {
                        ushort channel = (ushort)((data[0] & 0x0F) + 1);
                        return new ControlChangeMessage(channel, data[1], data[2]);
                    }
                case 0x0C:
                    {
                        ushort channel = (ushort)((data[0] & 0x0F) + 1);
                        return new ProgramChangeMessage(channel, data[1]);
                    }
                case 0x0F:
                    {
                        if (data[0] == 0xF0 && data.Last() == 0xF7)
                        {
                            return new SystemExclusiveMessage(data.Skip(1).TakeWhile(b => b != 0xF7).ToArray());
                        }
                        throw new MidiSerializerException("Unexpected system message format");
                    }
                // 0x0A Polyphony
                // 0x0D Channel pressure
                // 0x0E Pitch bend
                default:
                    throw new MidiSerializerException($"Deserializer not implemented for MIDI command: 0x{command:X2}");
            }
        }

        public byte[] Serialize(MidiMessage message)
        {
            switch (message.Class)
            {
                case MidiMessageClass.ChannelVoice:
                    return SerializeChannelVoice(message as ChannelVoiceMessage);
                default:
                    throw new MidiSerializerException($"Serializer not implemented for MIDI class: {message.Class}");
            }
        }

        private byte[] SerializeChannelVoice(ChannelVoiceMessage message)
        {
            int status = message.Channel - 1;
            switch (message.MessageType)
            {
                case ChannelVoiceMessageType.Note:
                    var noteMessage = message as NoteMessage;
                    byte[] notePacket = new byte[3];
                    var firstByte = (byte)(status | 0x80);
                    if (noteMessage.IsOn)
                    {
                        firstByte |= 0x10; // Note-ON has command = 0x90
                    }
                    notePacket[0] = firstByte;
                    notePacket[1] = (byte)noteMessage.Note;
                    notePacket[2] = (byte)noteMessage.Velocity;
                    return notePacket;
                default:
                    throw new MidiSerializerException($"Serializer not implemented for channel message {message.MessageType}");
            }
        }
    }
}
