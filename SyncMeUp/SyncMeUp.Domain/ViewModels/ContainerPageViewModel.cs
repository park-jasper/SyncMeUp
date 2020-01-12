using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using SyncMeUp.Domain.Commands;
using SyncMeUp.Domain.Domain;
using SyncMeUp.Domain.Model;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Domain.ViewModels
{
    public class ContainerPageViewModel : ViewModelBase
    {
        public IList<ContainerViewModel> Containers { get; set; } = new List<ContainerViewModel>();
        public ContainerViewModel CurrentContainer { get; set; }
        public bool IsContainerSelected { get; set; } = false;
        public ICommand CreateNewContainer { get; set; }
        public ICommand ToggleVisible { get; set; }

        public ContainerPageViewModel()
        {
            CreateNewContainer = new CommandForwarding(AddNewContainer);
            PropertyChanged += ContainerPagePropertyChanged;
        }

        public void ContainerPagePropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
                case nameof(CurrentContainer):
                    IsContainerSelected = CurrentContainer != null;
                    break;
            }
        }

        public void AddNewContainer(object sender)
        {
            var container = new SynchronizationContainer();
            var containerViewModel = new ContainerViewModel(
                Di.GetInstance<KnownClientsProvider>(),
                container
            );
            Containers.Add(containerViewModel);
            CurrentContainer = containerViewModel;
        }
    }
}