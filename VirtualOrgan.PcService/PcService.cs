using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtualOrgan.PcService.Archive;
using VirtualOrgan.PcService.Hauptwerk;

namespace VirtualOrgan.PcService
{
    internal sealed class PcService : IPcService, IDisposable
    {
        private readonly ILogger<PcService> logger;
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly IHauptwerkExeHelper exeHelper;
        private readonly IMidiArchiveHandler folder;

        private readonly IDisposable subscriptionToHauptwerkStatus;

        private PcStatus status;

        public PcService(
            ILogger<PcService> logger,
            IHauptwerkMidiInterface hauptwerk,
            IHauptwerkExeHelper exeHelper,
            IMidiArchiveHandler folder)
        {
            this.logger = logger;
            this.hauptwerk = hauptwerk;
            this.exeHelper = exeHelper;
            this.folder = folder;
            status = new PcStatus();
            subscriptionToHauptwerkStatus = hauptwerk.HauptwerkStatuses.Subscribe(
                hwStatus =>
                {
                    status.IsHauptwerkRunning = true;
                    status.IsHauptwerkAudioActive = hwStatus.IsHauptwerkAudioActive;
                    status.IsHauptwerkMidiActive = hwStatus.IsHauptwerkMidiActive;
                });
        }

        public void Dispose()
        {
            subscriptionToHauptwerkStatus.Dispose();
        }

        public PcStatus GetStatus()
        {
            return status;
        }

        public void PlayMidiFile(int id)
        {
            hauptwerk.StopMidiPlayback();
            folder.SetActive(id);
            hauptwerk.StartMidiPlayback();
        }

        public void ResetMidiAndAudio()
        {
            hauptwerk.ResetAudioAndMidi();
        }

        public void ShutdownPc()
        {
            hauptwerk.ShutDownComputer();
        }

        public void StartHauptwerk()
        {
            exeHelper.StartHauptwerk();
        }

        public void StopPlayback()
        {
            hauptwerk.StopMidiPlayback();
        }
    }
}
