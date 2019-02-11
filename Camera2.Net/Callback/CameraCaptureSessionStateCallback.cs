using System;
using Android.Hardware.Camera2;
using Android.Views;
using Camera2.Net.Wrappers;

namespace Camera2.Net.Callback
{
    public class CameraCaptureSessionStateCallback : CameraCaptureSession.StateCallback
    {
        private readonly Action<CameraCaptureSessionWrapper> _onActive;
        private readonly Action<CameraCaptureSessionWrapper> _onCaptureQueueEmpty;
        private readonly Action<CameraCaptureSessionWrapper> _onClosed;
        private readonly Action<CameraCaptureSessionWrapper> _onConfigured;
        private readonly Action<CameraCaptureSessionWrapper> _onConfigureFailed;
        private readonly Action<CameraCaptureSessionWrapper> _onReady;
        private readonly Action<CameraCaptureSessionWrapper, Surface> _onSurfacePrepared;

        public CameraCaptureSessionStateCallback(
            Action<CameraCaptureSessionWrapper> onActive = null,
            Action<CameraCaptureSessionWrapper> onCaptureQueueEmpty = null,
            Action<CameraCaptureSessionWrapper> onClosed = null,
            Action<CameraCaptureSessionWrapper> onConfigured = null,
            Action<CameraCaptureSessionWrapper> onConfigureFailed = null,
            Action<CameraCaptureSessionWrapper> onReady = null,
            Action<CameraCaptureSessionWrapper, Surface> onSurfacePrepared = null)
        {
            _onActive = onActive;
            _onCaptureQueueEmpty = onCaptureQueueEmpty;
            _onClosed = onClosed;
            _onConfigured = onConfigured;
            _onConfigureFailed = onConfigureFailed;
            _onReady = onReady;
            _onSurfacePrepared = onSurfacePrepared;
        }
        public override void OnActive(CameraCaptureSession session)
        {
            _onActive?.Invoke(new CameraCaptureSessionWrapper(session));
        }
        public override void OnCaptureQueueEmpty(CameraCaptureSession session)
        {
            _onCaptureQueueEmpty?.Invoke(new CameraCaptureSessionWrapper(session));
        }
        public override void OnClosed(CameraCaptureSession session)
        {
            _onClosed?.Invoke(new CameraCaptureSessionWrapper(session));
        }
        public override void OnConfigured(CameraCaptureSession session)
        {
            _onConfigured?.Invoke(new CameraCaptureSessionWrapper(session));
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            _onConfigureFailed?.Invoke(new CameraCaptureSessionWrapper(session));
        }
        public override void OnReady(CameraCaptureSession session)
        {
            _onReady?.Invoke(new CameraCaptureSessionWrapper(session));
        }
        public override void OnSurfacePrepared(CameraCaptureSession session, Surface surface)
        {
            _onSurfacePrepared?.Invoke(new CameraCaptureSessionWrapper(session), surface);
        }
    }
}