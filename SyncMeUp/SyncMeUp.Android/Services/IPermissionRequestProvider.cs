using System.Threading.Tasks;

namespace SyncMeUp.Droid.Services
{
    public interface IPermissionRequestProvider
    {
        Task<bool> CheckAndRequestPermissionAsync(string permission);
    }
}