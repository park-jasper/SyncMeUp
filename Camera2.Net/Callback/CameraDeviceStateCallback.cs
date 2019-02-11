using System;
using Android.Hardware.Camera2;
using Camera2.Net.Wrappers;

namespace Camera2.Net.Callback
{
    public class CameraDeviceStateCallback : CameraDevice.StateCallback
    {
        private readonly Action<CameraDeviceWrapper> _callbackOnOpened;
        private readonly Action<CameraDeviceWrapper, CameraError> _callbackOnError;
        private readonly Action<CameraDeviceWrapper> _callbackOnDisconnected;
        public CameraDeviceStateCallback(Action<CameraDeviceWrapper> callbackOnOpened, Action<CameraDeviceWrapper, CameraError> callbackOnError, Action<CameraDeviceWrapper> callbackOnDisconnected)
        {
            _callbackOnOpened = callbackOnOpened;
            _callbackOnError = callbackOnError;
            _callbackOnDisconnected = callbackOnDisconnected;
        }
        public override void OnDisconnected(CameraDevice camera)
        {
            _callbackOnDisconnected?.Invoke(new CameraDeviceWrapper(camera));
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            _callbackOnError?.Invoke(new CameraDeviceWrapper(camera), error);
        }

        public override void OnOpened(CameraDevice camera)
        {
            _callbackOnOpened?.Invoke(new CameraDeviceWrapper(camera));
        }
    }
}