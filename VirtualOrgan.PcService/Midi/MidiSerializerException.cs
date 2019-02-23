using System;
using System.Runtime.Serialization;

namespace VirtualOrgan.PcService.Midi
{
    [Serializable]
    public sealed class MidiSerializerException : Exception
    {
        public MidiSerializerException() { }

        public MidiSerializerException(string message)
            : base(message) { }

        public MidiSerializerException(string message, Exception innerException)
            : base(message, innerException) { }

        private MidiSerializerException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
