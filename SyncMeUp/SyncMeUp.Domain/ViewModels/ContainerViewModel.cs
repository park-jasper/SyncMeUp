using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Domain;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Domain.ViewModels
{
    public class ContainerViewModel : ViewModelBase
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public SynchronizationOptions SyncOptions { get; set; }
        public List<string> KnownPeers { get; set; }

        public ICommand PickFolder { get; set; }

        public IList<SyncOptionViewModel> AvailableSyncOptions { get; } = new List<SyncOptionViewModel> {
            new SyncOptionViewModel(SynchronizationOptions.Download),
            new SyncOptionViewModel(SynchronizationOptions.Upload),
            new SyncOptionViewModel(SynchronizationOptions.Download | SynchronizationOptions.Upload)
        };
        public int SyncOptionIndex { get; set; }

        public ContainerViewModel(KnownClientsProvider provider, SynchronizationContainer container)
        {
            Guid = container.Guid.ToString();
            Name = container.Name;
            RelativePath = container.RelativePath;
            SyncOptions = container.SyncOptions;
            KnownPeers = container.KnownPeers.Select(p => p.ToString()).ToList();

            PickFolder = new Commands.CommandForwarding(async e =>
            {
                var picker = Di.GetInstance<IFilePicker>();
                RelativePath = await picker.PickDirectory();
            });
            PropertyChanged += ContainerViewModel_PropertyChanged;
        }

        private void ContainerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SyncOptionIndex):
                    SyncOptions = AvailableSyncOptions[SyncOptionIndex].SyncOptions;
                    break;
            }
        }
    }
}