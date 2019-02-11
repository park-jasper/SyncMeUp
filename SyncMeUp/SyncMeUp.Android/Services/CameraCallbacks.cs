using System;
using Android.Hardware.Camera2;
using Android.Media;

namespace SyncMeUp.Droid.Services
{
    public class CameraDeviceCallback : CameraDevice.StateCallback
    {
        private readonly Action<CameraDevice> _callbackOnOpened;
        private readonly Action<CameraDevice, CameraError> _callbackOnError;
        private readonly Action<CameraDevice> _callbackOnDisconnected;
        public CameraDeviceCallback(Action<CameraDevice> callbackOnOpened, Action<CameraDevice, CameraError> callbackOnError, Action<CameraDevice> callbackOnDisconnected)
        {
            _callbackOnOpened = callbackOnOpened;
            _callbackOnError = callbackOnError;
            _callbackOnDisconnected = callbackOnDisconnected;
        }
        public override void OnDisconnected(CameraDevice camera)
        {
            _callbackOnDisconnected?.Invoke(camera);
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            _callbackOnError?.Invoke(camera, error);
        }

        public override void OnOpened(CameraDevice camera)
        {
            _callbackOnOpened?.Invoke(camera);
        }
    }
    public class CaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly Action<CameraCaptureSession> _onConfigured;
        private readonly Action<CameraCaptureSession> _onConfigureFailed;

        public CaptureSessionCallback(Action<CameraCaptureSession> onConfigured, Action<CameraCaptureSession> onConfigureFailed)
        {
            _onConfigured = onConfigured;
            _onConfigureFailed = onConfigureFailed;
        }
        public override void OnConfigured(CameraCaptureSession session)
        {
            _onConfigured?.Invoke(session);
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            _onConfigureFailed?.Invoke(session);
        }
    }

    public class CaptureCallback : CameraCaptureSession.CaptureCallback
    {
        private readonly Action<CameraCaptureSession, CaptureRequest, TotalCaptureResult> _onCaptureCompleted;
        private readonly Action<CameraCaptureSession, CaptureRequest, CaptureFailure> _onCaptureFailed;

        public CaptureCallback(
            Action<CameraCaptureSession, CaptureRequest, TotalCaptureResult> onCaptureCompleted,
            Action<CameraCaptureSession, CaptureRequest, CaptureFailure> onCaptureFailed)
        {
            _onCaptureCompleted = onCaptureCompleted;
            _onCaptureFailed = onCaptureFailed;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            _onCaptureCompleted?.Invoke(session, request, result);
        }
        public override void OnCaptureFailed(CameraCaptureSession session, CaptureRequest request, CaptureFailure failure)
        {
            _onCaptureFailed?.Invoke(session, request, failure);
        }
    }

    public class OnImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private readonly Action<ImageReader> _onImageAvailable;

        public OnImageAvailableListener(Action<ImageReader> onImageAvailable)
        {
            _onImageAvailable = onImageAvailable;
        }
        public void OnImageAvailable(ImageReader reader)
        {
            _onImageAvailable?.Invoke(reader);
        }
    }
}