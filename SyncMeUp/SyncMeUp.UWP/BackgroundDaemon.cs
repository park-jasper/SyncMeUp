using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Model;
using SyncMeUp.UWP.Services;

namespace SyncMeUp.UWP
{
    public class BackgroundDaemon : IBackgroundDaemon
    {
        private readonly ApplicationTrigger _serverApplicationTrigger = new ApplicationTrigger();

        public async Task Start(CommunicationRole role)
        {
            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();
            var allowedStatusList = new[]
            {
                BackgroundAccessStatus.AllowedSubjectToSystemPolicy,
                BackgroundAccessStatus.AlwaysAllowed
            };
            if (!allowedStatusList.Any(s => requestStatus == s))
            {
                return;
            }

            string taskEntryPoint = $"{nameof(SyncMeUp)}.{nameof(SyncMeUp.UWP)}.{nameof(SyncMeUp.UWP.BackgroundTask)}.";
            string taskName;
            IBackgroundTrigger trigger;
            switch (role)
            {
                case CommunicationRole.Server:
                    taskEntryPoint += nameof(BackgroundTask.ServerBackgroundTask);
                    trigger = _serverApplicationTrigger;
                    taskName = Constants.ServerBackgroundDaemonName;
                    break;

                case CommunicationRole.Client:
                default:
                    taskEntryPoint += nameof(BackgroundTask.ClientBackgroundTask);
                    trigger = new TimeTrigger(30, false);
                    taskName = Constants.ClientBackgroundDaemonName;
                    break;
            }

            var (guid, runningTask) = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == taskName);
            if (runningTask != null)
            {
                runningTask.Progress += OnProgress;
                if (role == CommunicationRole.Server)
                {
                    await _serverApplicationTrigger.RequestAsync();
                }
                return;
            }
            var builder = new BackgroundTaskBuilder
            {
                Name = taskName,
                CancelOnConditionLoss = false,
                IsNetworkRequested = true,
                TaskEntryPoint = taskEntryPoint,
            };

            builder.SetTrigger(trigger);
            var task = builder.Register();
            task.Completed += OnCompleted;
            task.Progress += OnProgress;

            if (role == CommunicationRole.Server)
            {
                await _serverApplicationTrigger.RequestAsync();
            }
        }

        private void OnProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            Trace.WriteLine($"Progress at {args.Progress}%");
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            Trace.WriteLine($"Task {args.InstanceId} completed");
        }

        public Task Stop(CommunicationRole role, bool force)
        {
            throw new NotImplementedException();
        }

        public Task<BackgroundDaemonStatus> GetStatus(CommunicationRole role)
        {
            throw new NotImplementedException();
        }

        public Task TriggerAction(CommunicationRole role)
        {
            throw new NotImplementedException();
        }

        public Task ClearJobs(CommunicationRole role, bool clearCurrentJobIfRunning)
        {
            throw new NotImplementedException();
        }
    }
}