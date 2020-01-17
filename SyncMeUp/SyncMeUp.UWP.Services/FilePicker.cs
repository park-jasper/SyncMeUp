using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.UWP.Services
{
    public class FilePicker : IFilePicker
    {
        public async Task<string> PickDirectory()
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            StorageFolder result = await picker.PickSingleFolderAsync();

            if (result != null && !StorageApplicationPermissions.FutureAccessList.CheckAccess(result))
            {
                StorageApplicationPermissions.FutureAccessList.Add(result);
            }

            return result?.Path;
        }
    }
}