using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.OS;
using Android.Views;
using MoreLinq;
using SyncMeUp.Services;

namespace SyncMeUp.Droid.Services
{
    public class DeviceCamera
    {
        private readonly Context _containingContext;
        public bool DeviceHasCamera => _containingContext.PackageManager.HasSystemFeature(PackageManager.FeatureCamera);

        //private CameraDeviceWrapper _camera;
        //private CameraCaptureSessionWrapper _captureSession;
        private Surface _surface;
        private TaskCompletionSource<byte[]> _currentTaskSource;

        private byte[] _currentPreviewImage;
        private readonly object _currentPreviewImageLock = new object();

        public DeviceCamera(Context containingContext)
        {
            _containingContext = containingContext;
        }
        //public async Task<bool> StartCapture()
        //{
        //    lock (_currentPreviewImageLock)
        //    {
        //        _currentPreviewImage = null;
        //    }
            //var manager = Camera2.Net.Wrappers.CameraManagerWrapper.GetCameraManager(_containingContext);
            //var cameraIds = manager.GetCameraIds();

            //var cameraIdToUse = cameraIds.OrderBy(id => id.Order).FirstOrDefault();
            //if (cameraIdToUse == null)
            //{
            //    return false;
            //}
            
            //var deviceResult = await manager.OpenCameraAsync(cameraIdToUse, device => { }, new Handler());
            //if (deviceResult.Successful)
            //{
            //    //_camera = deviceResult.Result;
            //    var largest = cameraIdToUse
            //        .StreamConfigurationMap
            //        .GetOutputSizes(ImageFormatType.Jpeg)
            //        .MaxBy(size => (long) size.Width * size.Height);
            //    var imageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, 2);
            //    imageReader.SetOnImageAvailableListener(new OnImageAvailableListener(reader =>
            //        {
            //            var image = reader.AcquireNextImage();
            //            var plane = image.GetPlanes()[0];
            //            var buffer = plane.Buffer;
            //            var byteResult = new byte[buffer.Remaining()];
            //            buffer.Get(byteResult);
            //            lock (_currentPreviewImageLock)
            //            {
            //                _currentPreviewImage = byteResult;
            //            }
            //        }),
            //        new Handler(message => { }));
            //    _surface = imageReader.Surface;
                //var sessionResult = await _camera.CreateCaptureSessionAsync(new List<Surface> { _surface }, new Handler());
                //if (sessionResult.Successful)
                //{
                //    _captureSession = sessionResult.Result;
                //    var requestBuilder = _camera.CreateCaptureRequest(CameraTemplate.Preview);
                //    requestBuilder.AddTarget(_surface);
                //    var request = requestBuilder.Build();
                //    var captureTask = new TaskCompletionSource<byte[]>();
                //    _currentTaskSource = captureTask;
                //    _captureSession.Capture(
                //        request,
                //        (s, req, res) => { },
                //        (s, req, fail) => { },
                //        new Handler());
                //}
                //else
                //{
                //    CloseCamera();
                //}
            //}
        //    return deviceResult.Successful;
        //}

        //private void CloseCamera()
        //{
        //    //_camera?.Close();
        //    //_camera?.Dispose();
        //    //_camera = null;
        //}

        //public void StopCapture()
        //{
        //    //_captureSession.AbortCaptures();
        //    //_captureSession.Close();
        //    //CloseCamera();
        //}

        //public async Task<byte[]> GetCurrentShotAsBitmap()
        //{
        //    while (true)
        //    {
        //        lock (_currentPreviewImageLock)
        //        {
        //            if (_currentPreviewImage != null && _currentPreviewImage.Length != 0)
        //            {
        //                var result = new byte[_currentPreviewImage.Length];
        //                Array.Copy(_currentPreviewImage, result, result.Length);
        //                return result;
        //            }
        //        }

        //        await Task.Delay(TimeSpan.FromMilliseconds(1000d / 60)); //Wait for 1/60th of a second to get a frame
        //    }

        //    //result.Get(CaptureResult.)
        //}
    }
}