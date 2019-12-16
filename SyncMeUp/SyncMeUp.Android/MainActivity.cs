//using System;
using System.Threading.Tasks;
using Android;
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
using Android.OS.Storage;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Java.IO;
using Java.Lang;
using Java.Lang.Reflect;
using SyncMeUp.Domain.Domain;

namespace SyncMeUp.Droid
{
    [Activity(Label = "SyncMeUp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IFilePicker,
        IPermissionRequestProvider
    {
        private TaskCompletionSource<string> _filePickerDoneTaskSource;
        private TaskCompletionSource<bool> _permissionRequestTaskSource;
        private const int FilePickerRequestCode = 7;
        private const int PermissionRequestId = 8;

        public static Context AppContext { get; private set; }
        public static Activity CurrentActivity { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Di.RegisterInstance<IQrCodeScanService>(new QrCodeScanService());
            Di.RegisterInstance<IUniqueIdentifierService>(
                new UniqueIdentifierService(() => ApplicationContext.ContentResolver));
            Di.RegisterType<IFileService, FileService>(true);
            Di.RegisterInstance<IFilePicker>(this);
            Di.RegisterInstance<IPermissionRequestProvider>(this);

            MobileBarcodeScanner.Initialize(Application);

            AppContext = this;
            CurrentActivity = this;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions,
                grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            switch (requestCode)
            {
                case PermissionRequestId:
                    var hasPermission = grantResults.Length > 0 && grantResults[0] == Permission.Granted;
                    Di.GetInstance<ISettingsProvider>()
                        .Store(Constants.ExternalStoragePermissionSettingsKey, hasPermission ? Constants.PermissionGranted : Constants.PermissionDenied);
                    _permissionRequestTaskSource?.SetResult(hasPermission);
                    _permissionRequestTaskSource = null;
                    break;
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case FilePickerRequestCode:
                    if (resultCode == Result.Ok)
                    {
                        var path = CustomFileUtil.GetFullPathFromTreeUri(data.Data, this);
                        _filePickerDoneTaskSource?.SetResult(path);
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

        

        public Task<bool> CheckAndRequestPermissionAsync(string permission)
        {
            if (ContextCompat.CheckSelfPermission(this, permission) != Permission.Granted)
            {
                if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
                {
                    //TODO display rationale
                    System.Console.WriteLine("Permission rationale");
                    return Task.FromResult(false);
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new[] { permission }, PermissionRequestId);
                    _permissionRequestTaskSource = new TaskCompletionSource<bool>();
                    return _permissionRequestTaskSource.Task;
                }
            }
            else
            {
                return Task.FromResult(true);
            }
        }

        private sealed class CustomFileUtil
        {
            private static string Tag = "TAG";
            private const string PrimaryVolumeName = "primary";

            public static string GetFullPathFromTreeUri(Android.Net.Uri treeUri, Context context)
            {
                if (treeUri == null)
                {
                    return null;
                }
                var volumePath = GetVolumePath(GetVolumeIdFromTreeUri(treeUri), context);
                if (string.IsNullOrEmpty(volumePath))
                {
                    return File.Separator;
                }
                if (volumePath.EndsWith(File.Separator))
                {
                    volumePath = volumePath.Substring(0, volumePath.Length - 1);
                }
                string documentPath = GetDocumentPathFromTreeUri(treeUri);
                if (documentPath.EndsWith(File.Separator))
                {
                    documentPath = documentPath.Substring(0, documentPath.Length - 1);
                }
                if (documentPath.Length > 0)
                {
                    if (documentPath.StartsWith(File.Separator))
                    {
                        return volumePath + documentPath;
                    }
                    else
                    {
                        return volumePath + File.Separator + documentPath;
                    }
                }
                else
                {
                    return null;
                }
            }

            public static string GetVolumePath(string volumeId, Context context)
            {
                if (Build.VERSION.SdkInt < Build.VERSION_CODES.Lollipop)
                {
                    return null;
                }
                try
                {
                    StorageManager storageManager = (StorageManager) context.GetSystemService(Context.StorageService);
                    var storageVolumeClass = Java.Lang.Class.ForName("android.os.storage.StorageVolume");
                    var getVolumeList = storageManager.Class.GetMethod("getVolumeList");
                    var getUuid = storageVolumeClass.GetMethod("getUuid");
                    var getPath = storageVolumeClass.GetMethod("getPath");
                    var isPrimary = storageVolumeClass.GetMethod("isPrimary");
                    var result = getVolumeList.Invoke(storageManager);

                    var length = Array.GetLength(result);
                    for (int i = 0; i < length; i++)
                    {
                        var storageVolumeElement = Array.Get(result, i);
                        var uuid = (string) getUuid.Invoke(storageVolumeElement);
                        var primary = (bool) isPrimary.Invoke(storageVolumeElement);

                        if (primary && PrimaryVolumeName == volumeId)
                        {
                            return (string) getPath.Invoke(storageVolumeElement);
                        }
                        if (uuid != null && uuid == volumeId)
                        {
                            return (string) getPath.Invoke(storageVolumeElement);
                        }
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return null;
                }
            }

            public static string GetVolumeIdFromTreeUri(Android.Net.Uri treeUri)
            {
                if (!DocumentsContract.IsTreeUri(treeUri))
                {
                    return null;
                }
                var docId = DocumentsContract.GetTreeDocumentId(treeUri);
                var split = docId.Split(':');
                return split[0];
            }

            public static string GetDocumentPathFromTreeUri(Android.Net.Uri treeUri)
            {
                if (!DocumentsContract.IsTreeUri(treeUri))
                {
                    return null;
                }
                var docId = DocumentsContract.GetTreeDocumentId(treeUri);
                var split = docId.Split(':');
                if (split.Length >= 2 && !string.IsNullOrEmpty(split[1]))
                {
                    return split[1];
                }
                return File.Separator;
            }
        }
    }
}