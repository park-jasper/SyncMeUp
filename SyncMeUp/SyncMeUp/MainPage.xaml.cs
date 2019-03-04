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
using Xamarin.Forms;
//using ZXing;
//using ZXing.Mobile;

namespace SyncMeUp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();

            Di.RegisterInstance(this);
        }
    }
}