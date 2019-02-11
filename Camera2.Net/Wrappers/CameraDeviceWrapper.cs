using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Views;
using Camera2.Net.Callback;

namespace Camera2.Net.Wrappers
{
    public class CameraDeviceWrapper : IDisposable
    {
        private readonly CameraDevice _internalDevice;
        public bool HasCameraDevice => _internalDevice != null;
        internal CameraDeviceWrapper(CameraDevice internalDevice)
        {
            _internalDevice = internalDevice;
        }

        public void Close()
        {
            _internalDevice.Close();
        }

        public void CreateCaptureSession(
            IList<Surface> outputs, 
            Handler handler,
            Action<CameraCaptureSessionWrapper> onActive = null, 
            Action<CameraCaptureSessionWrapper> onCaptureQueueEmpty = null,
            Action<CameraCaptureSessionWrapper> onClosed = null,
            Action<CameraCaptureSessionWrapper> onConfigured = null,
            Action<CameraCaptureSessionWrapper> onConfigureFailed = null,
            Action<CameraCaptureSessionWrapper> onReady = null,
            Action<CameraCaptureSessionWrapper, Surface> onSurfacePrepared = null)
        {
            _internalDevice.CreateCaptureSession(
                outputs,
                new CameraCaptureSessionStateCallback(
                    onActive,
                    onCaptureQueueEmpty,
                    onClosed,
                    onConfigured,
                    onConfigureFailed,
                    onReady,
                    onSurfacePrepared),
                handler);
        }

        public Task<CallbackResult<CameraCaptureSessionWrapper>> CreateCaptureSessionAsync(
            IList<Surface> outputs,
            Handler handler,
            Action<CameraCaptureSessionWrapper> onClosed = null)
        {
            var taskSource = new TaskCompletionSource<CallbackResult<CameraCaptureSessionWrapper>>();
            CreateCaptureSession(
                outputs,
                handler,
                onConfigured: session =>
                    taskSource.TrySetResult(
                        new CallbackResult<CameraCaptureSessionWrapper> { Successful = true, Result = session }),
                onConfigureFailed: session =>
                    taskSource.TrySetResult(
                        new CallbackResult<CameraCaptureSessionWrapper> { Successful = false, Result = session }),
                onClosed: onClosed);
            return taskSource.Task;
        }

        public CaptureRequest.Builder CreateCaptureRequest(CameraTemplate template)
        {
            return _internalDevice.CreateCaptureRequest(template);
        }

        public void Dispose()
        {
            _internalDevice.Dispose();
        }
    }
}