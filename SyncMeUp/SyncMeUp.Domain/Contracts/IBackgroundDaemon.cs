using System.Threading.Tasks;
using SyncMeUp.Domain.Model;

namespace SyncMeUp.Domain.Contracts
{
    public interface IBackgroundDaemon
    {
        Task Start(CommunicationRole role);
        Task Stop(CommunicationRole role, bool force);
        Task<BackgroundDaemonStatus> GetStatus(CommunicationRole role);
        Task TriggerAction(CommunicationRole role);
        Task ClearJobs(CommunicationRole role, bool clearCurrentJobIfRunning);
    }
}