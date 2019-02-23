using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualOrgan.PcService.Midi
{
    internal interface IMidiMessageVisitor
    {
        void Visit(NoteMessage noteMessage);
        void Visit(ControlChangeMessage controlChangeMessage);
        void Visit(ProgramChangeMessage programChangeMessage);
        void Visit(SystemExclusiveMessage systemExclusiveMessage);
    }
}
