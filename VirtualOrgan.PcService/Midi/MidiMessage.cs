namespace VirtualOrgan.PcService.Midi
{
    enum MidiMessageClass
    {
        ChannelVoice,
        ChannelMode,
        System,
    }

    enum ChannelVoiceMessageType
    {
        Note,
        Polyphonic,
        ControlChange,
        ProgramChange,
        ChannelPressure,
        PitchBendChange,
    }

    abstract class MidiMessage
    {
        public MidiMessageClass Class { get; }

        protected MidiMessage(MidiMessageClass messageClass)
        {
            Class = messageClass;
        }

        public abstract void Accept(IMidiMessageVisitor visitor);
    }

    abstract class ChannelVoiceMessage : MidiMessage
    {
        public ChannelVoiceMessageType MessageType { get; }

        /// <summary>
        /// Channel number (1-128)
        /// </summary>
        public ushort Channel { get; set; }

        protected ChannelVoiceMessage(ChannelVoiceMessageType messageType, ushort channel)
            : base(MidiMessageClass.ChannelVoice)
        {
            MessageType = messageType;
            Channel = channel;
        }
    }

    sealed class NoteMessage : ChannelVoiceMessage
    {
        /// <summary>
        /// Note number (0-127)
        /// </summary>
        public ushort Note { get; set; }

        public ushort Velocity { get; set; }

        public bool IsOn { get; set; }

        public NoteMessage(ushort channel, ushort note, bool isOn = true)
            : base(ChannelVoiceMessageType.Note, channel)
        {
            Note = note;
            Velocity = 64;
            IsOn = isOn;
        }

        public override void Accept(IMidiMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    sealed class ControlChangeMessage : ChannelVoiceMessage
    {
        /// <summary>
        /// Control number (0-119)
        /// </summary>
        public ushort Control { get; set; }

        /// <summary>
        /// Value of the control
        /// </summary>
        public ushort Value { get; set; }

        public ControlChangeMessage(ushort channel, ushort control, ushort value)
            : base(ChannelVoiceMessageType.ControlChange, channel)
        {
            Control = control;
            Value = value;
        }

        public override void Accept(IMidiMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    sealed class ProgramChangeMessage : ChannelVoiceMessage
    {
        /// <summary>
        /// Program number (0-127)
        /// </summary>
        public ushort Program { get; set; }

        public ProgramChangeMessage(ushort channel, ushort program)
            : base(ChannelVoiceMessageType.ControlChange, channel)
        {
            Program = program;
        }

        public override void Accept(IMidiMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    sealed class SystemExclusiveMessage : MidiMessage
    {
        public byte[] Data { get; set; }

        public SystemExclusiveMessage(byte[] data)
            : base(MidiMessageClass.System)
        {
            Data = data;
        }

        public override void Accept(IMidiMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
