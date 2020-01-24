using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Work;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Model;

namespace SyncMeUp.Droid.Services
{
    public class BackgroundDaemon : IBackgroundDaemon
    {
        private PeriodicWorkRequest _request;
        private IOperation _operation;
        private readonly Context _context;

        private readonly Constraints _constraints;

        public BackgroundDaemon(Context context)
        {
            _context = context;
            _constraints = new Constraints.Builder()
                .SetRequiredNetworkType(NetworkType.Connected)
                .SetRequiresBatteryNotLow(true)
                .Build();
        }
        public async Task Start(CommunicationRole role)
        {
            _request = new PeriodicWorkRequest.Builder(typeof(BackgroundWorkerTask), TimeSpan.FromMinutes(30))
                .SetConstraints(_constraints)
                .Build();
            _operation = WorkManager.Instance.Enqueue(_request);
        }

        public async Task Stop(CommunicationRole role, bool force)
        {
            _request.UnregisterFromRuntime();
        }

        public Task<BackgroundDaemonStatus> GetStatus(CommunicationRole role)
        {
            var state = _operation.State;
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

    internal class BackgroundWorkerTask : Worker
    {
        public BackgroundWorkerTask(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            throw new NotImplementedException();
        }
    }
}