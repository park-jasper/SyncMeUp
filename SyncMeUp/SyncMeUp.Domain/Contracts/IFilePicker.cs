using System.Threading.Tasks;

namespace SyncMeUp.Domain.Contracts
{
    public interface IFilePicker
    {
        Task<string> PickDirectory();
    }
}