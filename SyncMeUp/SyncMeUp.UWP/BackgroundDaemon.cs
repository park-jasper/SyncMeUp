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
        public async Task Start()
        {
            var (guid, runningTask) = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == Constants.BackgroundDaemonName);
            if (runningTask != null)
            {
                runningTask.Unregister(true);
                //runningTask.Progress += OnProgress;
                return;
            }
            var builder = new BackgroundTaskBuilder
            {
                Name = Constants.BackgroundDaemonName,
                CancelOnConditionLoss = false,
                IsNetworkRequested = true,
                TaskEntryPoint = $"{nameof(SyncMeUp)}.{nameof(SyncMeUp.UWP)}.{nameof(SyncMeUp.UWP.BackgroundTask)}.{nameof(BackgroundTask.BackgroundTask)}"
            };
            var appTrigger = new ApplicationTrigger();
            builder.SetTrigger(appTrigger); 
            var task = builder.Register();
            task.Completed += OnCompleted;
            task.Progress += OnProgress;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[Constants.LocalSettingsBackgroundTaskModeKey] = Constants.BackgroundTaskModeServer;
            await appTrigger.RequestAsync();
        }

        private void OnProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            Trace.WriteLine($"Progress at {args.Progress}%");
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            Trace.WriteLine($"Task {args.InstanceId} completed");
        }

        public Task Stop(bool force)
        {
            throw new System.NotImplementedException();
        }

        public Task<BackgroundDemonStatus> GetStatus()
        {
            throw new System.NotImplementedException();
        }

        public Task Queue(BackgroundJob job)
        {
            throw new System.NotImplementedException();
        }

        public Task ClearJobs(bool clearCurrentJobIfRunning)
        {
            throw new System.NotImplementedException();
        }
    }
}