using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtualOrgan.PcService.Hauptwerk;

namespace VirtualOrgan.PcService
{
    internal sealed class PcService : IPcService, IDisposable
    {
        private readonly IDisposable subscriptionToHauptwerkStatus;
        private readonly IHauptwerkMidiInterface hauptwerk;
        private readonly Dictionary<Operation, ITaskFactory> taskFactories;
        private readonly ILogger<PcService> logger;
        private PcStatus status;
        private ITaskWrapper currentTask;

        public PcService(
            IHauptwerkMidiInterface hauptwerk,
            IEnumerable<ITaskFactory> taskFactories,
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
            this.taskFactories = taskFactories.ToDictionary(t => t.Operation);
            this.logger = logger;
            RestartHauptwerk();
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
            RunTask(Operation.RestartHauptwerk);
        }

        public void ShutdownPc()
        {
            RunTask(Operation.QuitAndShutdown);
        }

        private void RunTask(Operation operation)
        {
            if (currentTask != null)
            {
                logger.LogDebug("Dispose running task");
                currentTask.Dispose();
                currentTask = null;
            }
            if (taskFactories.TryGetValue(operation, out ITaskFactory factory))
            {
                currentTask = factory.Create();
                currentTask.Start();
            }
            else
            {
                logger.LogWarning("Missing task for operation {0}", operation);
            }
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
