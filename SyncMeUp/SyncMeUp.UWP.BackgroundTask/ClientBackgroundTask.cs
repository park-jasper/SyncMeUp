using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using SyncMeUp.UWP.Services;

namespace SyncMeUp.UWP.BackgroundTask
{
    public sealed class ClientBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var modeString = localSettings.Values[Constants.LocalSettingsBackgroundTaskModeKey];
            switch (modeString)
            {
                case Constants.BackgroundTaskModeServer:
                    //TODO do server running
                    break;
                case Constants.BackgroundTaskModeClient:
                    //TODO do client running
                    break;
            }
            _deferral = taskInstance.GetDeferral();
            for (uint i = 0; i <= 100; i += 1)
            {
                taskInstance.Progress = i;
                System.Diagnostics.Debug.WriteLine($"Some progress {i}%");
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
            taskInstance.Progress = 5;
            _deferral.Complete();
            return;
        }
    }
}
