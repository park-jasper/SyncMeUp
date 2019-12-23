using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.GTK.Services
{
    public class FilePicker : IFilePicker
    {
        public Task<string> PickDirectory()
        {
            throw new System.NotImplementedException();
        }
    }
}