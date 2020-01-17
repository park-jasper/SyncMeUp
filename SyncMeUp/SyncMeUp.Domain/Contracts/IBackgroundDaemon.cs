using System.Threading.Tasks;
using SyncMeUp.Domain.Model;

namespace SyncMeUp.Domain.Contracts
{
    public interface IBackgroundDaemon
    {
        Task Start();
        Task Stop(bool force);
        Task<BackgroundDemonStatus> GetStatus();
        Task Queue(BackgroundJob job);
        Task ClearJobs(bool clearCurrentJobIfRunning);
    }
}