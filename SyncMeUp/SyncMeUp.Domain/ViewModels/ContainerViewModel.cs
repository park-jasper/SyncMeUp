using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using SyncMeUp.Domain.Commands;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Domain;
using SyncMeUp.Domain.Model;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Domain.ViewModels
{
    public class ContainerViewModel : ViewModelBase
    {
        private SynchronizationContainer _container;
        public string Guid { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public List<string> KnownPeers { get; set; }

        public ICommand PickFolder { get; set; }

        public bool Upload { get; set; }
        public bool Download { get; set; }
        public bool CanEditFiles { get; set; }
        public bool CanDeleteFiles { get; set; }
        public bool CanAddFiles { get; set; }

        public ICommand ToggleUpload { get; }
        public ICommand ToggleDownload { get; }

        public bool ShowHostOnP2PCheckbox { get; set; }
        public bool HostOnP2P { get; set; }
        public ICommand ToggleHostOnP2P { get; }

        public IList<CommunicationRoleViewModel> CommunicationRoles { get; } = new List<CommunicationRoleViewModel>
        {
            new CommunicationRoleViewModel(CommunicationRole.Server),
            new CommunicationRoleViewModel(CommunicationRole.Client),
            new CommunicationRoleViewModel(CommunicationRole.P2PClient)
        };
        public CommunicationRoleViewModel SelectedCommunicationRole { get; set; }

        public ContainerViewModel(KnownClientsProvider provider, SynchronizationContainer container)
        {
            _container = container;

            Guid = container.Guid.ToString();
            Name = container.Name;
            RelativePath = container.Path;
            KnownPeers = container.KnownPeers.Select(p => p.ToString()).ToList();

            Upload = _container.SyncModes.Upload;
            Download = _container.SyncModes.Download;

            if (_container.OwnCommunicationRole == CommunicationRole.P2PClientPassive)
            {
                SelectedCommunicationRole =
                    CommunicationRoles.FirstOrDefault(vm => vm.Role == CommunicationRole.P2PClient);
                HostOnP2P = false;
            }
            else
            {
                SelectedCommunicationRole =
                    CommunicationRoles.FirstOrDefault(vm => vm.Role == _container.OwnCommunicationRole);
                if (_container.OwnCommunicationRole == CommunicationRole.P2PClient)
                {
                    HostOnP2P = true;
                }
                else
                {
                    HostOnP2P = GetDefaultHostOnP2P();
                }
            }
            

            PickFolder = new Commands.CommandForwarding(async e =>
            {
                var picker = Di.GetInstance<IFilePicker>();
                RelativePath = await picker.PickDirectory();
            });
            ToggleUpload = new CommandForwarding(e => Upload = !Upload);
            ToggleDownload = new CommandForwarding(e => Download = !Download);
            ToggleHostOnP2P = new CommandForwarding(e => HostOnP2P = !HostOnP2P);
            PropertyChanged += ContainerViewModel_PropertyChanged;
        }

        private void ContainerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Upload):
                    _container.SyncModes.Upload = Upload;
                    break;
                case nameof(Download):
                    _container.SyncModes.Download = Upload;
                    break;
                case nameof(SelectedCommunicationRole):
                    if (SelectedCommunicationRole == null)
                    {
                        break;
                    }
                    if (SelectedCommunicationRole.Role == CommunicationRole.P2PClient ||
                        SelectedCommunicationRole.Role == CommunicationRole.P2PClientPassive)
                    {
                        if (HostOnP2P)
                        {
                            _container.OwnCommunicationRole = CommunicationRole.P2PClient;
                        }
                        else
                        {
                            _container.OwnCommunicationRole = CommunicationRole.P2PClientPassive;
                        }
                        ShowHostOnP2PCheckbox = true;
                    }
                    else
                    {
                        _container.OwnCommunicationRole = SelectedCommunicationRole.Role;
                        ShowHostOnP2PCheckbox = false;
                        HostOnP2P = GetDefaultHostOnP2P();
                    }
                    break;
                case nameof(HostOnP2P):
                    if (_container.OwnCommunicationRole == CommunicationRole.P2PClientPassive && HostOnP2P)
                    {
                        _container.OwnCommunicationRole = CommunicationRole.P2PClient;
                    }
                    else if (_container.OwnCommunicationRole == CommunicationRole.P2PClientPassive && !HostOnP2P)
                    {
                        _container.OwnCommunicationRole = CommunicationRole.P2PClientPassive;
                    }
                    break;
            }
        }

        public bool GetDefaultHostOnP2P() => false;
    }
}