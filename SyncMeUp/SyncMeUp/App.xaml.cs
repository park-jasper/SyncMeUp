using System;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Services;
using SyncMeUp.Services;
using SyncMeUp.Services.Database;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SyncMeUp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Di.RegisterType<ISecureStorageProvider, SecureStorageProvider>(true);
            Di.RegisterType<ISettingsProvider, SettingsProvider>(true);
            Di.RegisterInstance<IBulkStorageProvider>(new BulkStorageProvider("containers.sqlite"));

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
