using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualOrgan.PcService.Hauptwerk
{
    public interface IHauptwerkExeHelper
    {
        bool IsHauptwerkRunning();
        void StartHauptwerk();
    }
}
