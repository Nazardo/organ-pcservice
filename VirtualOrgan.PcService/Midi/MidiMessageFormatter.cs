using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualOrgan.PcService.Midi
{
    internal sealed class MidiMessageFormatter : IMidiMessageVisitor
    {
        private readonly StringBuilder sb = new StringBuilder();
        private bool first = true;

        public string GetString() => sb.ToString();

        public void Visit(NoteMessage noteMessage)
        {
            HandleIfFirst();
            VisitChannelVoiceMessage(noteMessage);
            sb.AppendFormat("Note {0}: {1} (vel: {2})",
                noteMessage.Note,
                noteMessage.IsOn ? "ON" : "OFF",
                noteMessage.Velocity);
        }

        public void Visit(ControlChangeMessage controlChangeMessage)
        {
            HandleIfFirst();
            VisitChannelVoiceMessage(controlChangeMessage);
            sb.AppendFormat("Control {0}: {1}",
                controlChangeMessage.Control,
                controlChangeMessage.Value);
        }

        public void Visit(ProgramChangeMessage programChangeMessage)
        {
            HandleIfFirst();
            VisitChannelVoiceMessage(programChangeMessage);
            sb.AppendFormat("Program {0}", programChangeMessage.Program);
        }

        public void Visit(SystemExclusiveMessage systemExclusiveMessage)
        {
            HandleIfFirst();
            sb.AppendFormat("SYSEX ({0} B)", systemExclusiveMessage.Data.Length);
            foreach (var b in systemExclusiveMessage.Data)
            {
                sb.AppendFormat(" {0:X2}", b);
            }
        }

        private void VisitChannelVoiceMessage(ChannelVoiceMessage message)
        {
            sb.AppendFormat("ChannelVoice {0} (CH {1}) ", message.MessageType, message.Channel);
        }

        private void HandleIfFirst()
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.AppendLine();
            }
        }
    }
}
