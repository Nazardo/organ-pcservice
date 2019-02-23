using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VirtualOrgan.PcService.Hauptwerk
{
    internal sealed class HauptwerkExeHelper : IHauptwerkExeHelper
    {
        private readonly string hauptwerkPath;
        private readonly string processFriendlyName;
        private readonly ILogger<HauptwerkExeHelper> logger;

        public HauptwerkExeHelper(
            ILogger<HauptwerkExeHelper> logger,
            IOptions<HauptwerkConfiguration> config)
        {
            this.logger = logger;
            hauptwerkPath = config.Value.HauptwerkExePath;
            processFriendlyName = Path.GetFileNameWithoutExtension(hauptwerkPath);
        }

        public bool IsHauptwerkRunning()
        {
            return InternalIsHauptwerkRunning();
        }

        public void StartHauptwerk()
        {
            if (!InternalIsHauptwerkRunning())
            {
                Process.Start(hauptwerkPath);
            }
        }

        private bool InternalIsHauptwerkRunning()
        {
            var processes = Process.GetProcessesByName(processFriendlyName);
            if (logger.IsEnabled(LogLevel.Debug))
            {
                if (processes.Length == 0)
                {
                    logger.LogDebug("No Hauptwerk process (\"{0}\") found", processFriendlyName);
                }
                else
                {
                    foreach (var p in processes)
                    {
                        logger.LogDebug("Hauptwerk (\"{0}\") found at PID {1}", processFriendlyName, p.Id);
                    }
                }
            }
            return processes.Length > 0;
        }
    }
}
