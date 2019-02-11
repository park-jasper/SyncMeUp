using System;
using Android.Hardware.Camera2;
using Camera2.Net.Wrappers;

namespace Camera2.Net.Callback
{
    public class CameraCaptureSessionCaptureCallback : CameraCaptureSession.CaptureCallback
    {
        private readonly Action<CameraCaptureSessionWrapper, CaptureRequest, TotalCaptureResult> _onCaptureCompleted;
        private readonly Action<CameraCaptureSessionWrapper, CaptureRequest, CaptureFailure> _onCaptureFailed;

        public CameraCaptureSessionCaptureCallback(
            Action<CameraCaptureSessionWrapper, CaptureRequest, TotalCaptureResult> onCaptureCompleted,
            Action<CameraCaptureSessionWrapper, CaptureRequest, CaptureFailure> onCaptureFailed)
        {
            _onCaptureCompleted = onCaptureCompleted;
            _onCaptureFailed = onCaptureFailed;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            _onCaptureCompleted?.Invoke(new CameraCaptureSessionWrapper(session), request, result);
        }
        public override void OnCaptureFailed(CameraCaptureSession session, CaptureRequest request, CaptureFailure failure)
        {
            _onCaptureFailed?.Invoke(new CameraCaptureSessionWrapper(session), request, failure);
        }
    }
}