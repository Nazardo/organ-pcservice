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
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly IAudioCardHelper audioCardHelper;
        private readonly IHauptwerkExeHelper exeHelper;
        private readonly IMidiArchiveHandler folder;

        private readonly IDisposable subscriptionToHauptwerkStatus;

        private PcStatus status;

        public PcService(
            IHauptwerkMidiInterface hauptwerk,
            IAudioCardHelper audioCardHelper,
            IHauptwerkExeHelper exeHelper,
            IMidiArchiveHandler folder)
        {
            this.hauptwerk = hauptwerk;
            this.audioCardHelper = audioCardHelper;
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

        public async Task StartHauptwerk()
        {
            if (await audioCardHelper.IsAudioCardActive())
            {
                exeHelper.StartHauptwerk();
            }
        }

        public void StopPlayback()
        {
            hauptwerk.StopMidiPlayback();
        }
    }
}
