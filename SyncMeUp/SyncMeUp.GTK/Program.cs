using System;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Services;
using SyncMeUp.GTK.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace SyncMeUp.GTK
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();

            Di.RegisterType<IUniqueIdentifierService, UniqueIdentifierService>(true);
            Di.RegisterType<IFilePicker, FilePicker>(true);
            Di.RegisterType<IFileService, FileService>(true);

            var app = new App();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("Sync Me Up!");
            window.Show();

            Gtk.Application.Run();
        }
    }
}