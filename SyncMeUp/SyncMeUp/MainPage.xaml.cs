using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using QRCoder;
using SyncMeUp.Domain.Commands;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Cryptography;
using SyncMeUp.Domain.Networking;
using SyncMeUp.Domain.Services;
using SyncMeUp.Domain.ViewModels;
using SyncMeUp.Pages;
using Xamarin.Forms;
//using ZXing;
//using ZXing.Mobile;

namespace SyncMeUp
{
    public partial class MainPage : MasterDetailPage
    {
        public MainViewModel InnerBindingContext { get; set; }

        public ICommand NavigateToContainers { get; private set; }
        public ICommand Return { get; private set; }

        private readonly NavigationPage _navigation;
        public MainPage()
        {
            InitializeComponent();
            InnerBindingContext = new MainViewModel();
            BindingContext = this;

            NavigateToContainers = new CommandForwarding(Nav);
            Return = new CommandForwarding(sender => _navigation.PopAsync());
            Master = new MainNavigationPage() { BindingContext = this };
            Detail = _navigation = new NavigationPage(new MainContentPage() {BindingContext = InnerBindingContext});

            Di.RegisterInstance(this);
        }

        private void Nav(object sender)
        {
            _navigation.PushAsync(new ContainerPage() {BindingContext = new ContainerPageViewModel()});
        }
    }
}