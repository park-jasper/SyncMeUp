using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Services;
using SyncMeUp.Droid.Services;
using ZXing.Mobile;
using Android.Content;
using Android.Provider;

namespace SyncMeUp.Droid
{
    [Activity(Label = "SyncMeUp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IFilePicker
    {
        private TaskCompletionSource<string> _filePickerDoneTaskSource;
        private const int FilePickerRequestCode = 7;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            Di.RegisterInstance<IQrCodeScanService>(new QrCodeScanService());
            Di.RegisterInstance<IUniqueIdentifierService>(
                new UniqueIdentifierService(() => ApplicationContext.ContentResolver));
            Di.RegisterType<IFileService, FileService>(true);
            Di.RegisterInstance<IFilePicker>(this);
            
            MobileBarcodeScanner.Initialize(Application);

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case FilePickerRequestCode:
                    if (resultCode == Result.Ok)
                    {
                        var documentUri = DocumentsContract.GetTreeDocumentId(data.Data);
                        _filePickerDoneTaskSource?.SetResult(documentUri);
                    }
                    else
                    {
                        _filePickerDoneTaskSource?.SetResult(null);
                    }
                    _filePickerDoneTaskSource = null;
                    break;
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        public Task<string> PickDirectory()
        {
            var pickingIntent = new Intent(Intent.ActionOpenDocumentTree);
            pickingIntent.AddCategory(Intent.CategoryDefault);
            _filePickerDoneTaskSource = new TaskCompletionSource<string>();

            //Application.Context.StartActivity(pickingIntent);
            //StartActivity(pickingIntent);
            StartActivityForResult(pickingIntent, FilePickerRequestCode);
            return _filePickerDoneTaskSource.Task;
        }
    }
}