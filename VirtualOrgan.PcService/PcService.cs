using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VirtualOrgan.PcService.Hauptwerk;

namespace VirtualOrgan.PcService
{
    internal sealed class PcService : IPcService, IDisposable
    {
        private readonly IDisposable subscriptionToHauptwerkStatus;
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<PcService> logger;
        private PcStatus status;
        private ITaskWrapper currentTask;

        public PcService(
            IHauptwerkMidiInterface hauptwerk,
            IServiceProvider serviceProvider,
            ILogger<PcService> logger)
        {
            status = new PcStatus();
            subscriptionToHauptwerkStatus = hauptwerk.HauptwerkStatuses.Subscribe(
                hwStatus => status = new PcStatus()
                {
                    IsHauptwerkRunning = true,
                    IsHauptwerkAudioActive = hwStatus.IsHauptwerkAudioActive,
                    IsHauptwerkMidiActive = hwStatus.IsHauptwerkMidiActive
                });
            this.hauptwerk = hauptwerk;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            RunTask(serviceProvider.GetRequiredService<RestartHauptwerkTask>());
        }

        public void Dispose()
        {
            currentTask?.Dispose();
            subscriptionToHauptwerkStatus.Dispose();
        }

        public Task<PcStatus> GetStatusAsync()
        {
            return Task.FromResult(status);
        }

        public void ResetMidiAndAudio()
        {
            logger.LogDebug("ResetMidiAndAudio");
            hauptwerk.ResetMidiAndAudio();
        }

        public void RestartHauptwerk()
        {
            RunTask(serviceProvider.GetRequiredService<RestartHauptwerkTask>());
        }

        public void ShutdownPc()
        {
            RunTask(serviceProvider.GetRequiredService<QuitAndShutdownTask>());
        }

        private void RunTask(ITaskWrapper task)
        {
            if (currentTask != null)
            {
                logger.LogDebug("Dispose running task");
                currentTask.Dispose();
                currentTask = null;
            }
            currentTask = task;
            task.Task.Start();
        }

        public void PlayMidiFile(int id)
        {
            // to be implemented
        }

        public void StopPlayback()
        {
            // to be implemented
        }
    }
}
