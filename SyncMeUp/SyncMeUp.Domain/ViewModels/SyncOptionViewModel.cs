using SyncMeUp.Domain.Domain;

namespace SyncMeUp.Domain.ViewModels
{
    public class SyncOptionViewModel : ViewModelBase
    {
        public SynchronizationOptions SyncOptions { get; set; }

        public string Name
        {
            get
            {
                switch (SyncOptions)
                {
                    case SynchronizationOptions.Download:
                        return "Download";
                    case SynchronizationOptions.Upload:
                        return "Upload";
                    case SynchronizationOptions.Download | SynchronizationOptions.Upload:
                        return "Sync";
                    default:
                        SyncOptions = SynchronizationOptions.Download;
                        return "Download";
                }
            }
        }

        public SyncOptionViewModel(SynchronizationOptions option)
        {
            SyncOptions = option;
        }
    }
}