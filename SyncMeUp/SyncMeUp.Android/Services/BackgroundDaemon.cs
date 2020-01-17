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
        public async Task Start()
        {
            _request = new PeriodicWorkRequest.Builder(typeof(BackgroundWorkerTask), TimeSpan.FromMinutes(30))
                .SetConstraints(_constraints)
                .Build();
            _operation = WorkManager.Instance.Enqueue(_request);
        }

        public async Task Stop(bool force)
        {
            _request.UnregisterFromRuntime();
        }

        public Task<BackgroundDemonStatus> GetStatus()
        {
            var state = _operation.State;
            throw new NotImplementedException();
        }

        public Task Queue(BackgroundJob job)
        {
            throw new NotImplementedException();
        }

        public Task ClearJobs(bool clearCurrentJobIfRunning)
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