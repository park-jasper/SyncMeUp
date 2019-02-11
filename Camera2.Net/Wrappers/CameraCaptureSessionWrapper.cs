using System;
using System.Threading.Tasks;
using Android.Hardware.Camera2;
using Android.OS;
using Camera2.Net.Callback;

namespace Camera2.Net.Wrappers
{
    public class CameraCaptureSessionWrapper
    {
        private readonly CameraCaptureSession _internalSession;
        public bool HasCameraCaptureSession => _internalSession != null;

        internal CameraCaptureSessionWrapper(CameraCaptureSession internalSession)
        {
            _internalSession = internalSession;
        }

        public void AbortCaptures()
        {
            _internalSession.AbortCaptures();
        }

        public void Close()
        {
            _internalSession.Close();
        }

        public void Capture(
            CaptureRequest request,
            Action<CameraCaptureSessionWrapper, CaptureRequest, TotalCaptureResult> onCaptureCompleted,
            Action<CameraCaptureSessionWrapper, CaptureRequest, CaptureFailure> onCaptureFailed,
            Handler handler)
        {
            _internalSession.Capture(request,
                new CameraCaptureSessionCaptureCallback(onCaptureCompleted, onCaptureFailed),
                handler);
        }

        public Task<CallbackResult<TotalCaptureResult, CaptureFailure>> CaptureAsync(
            CaptureRequest request,
            Handler handler)
        {
            var taskSource = new TaskCompletionSource<CallbackResult<TotalCaptureResult, CaptureFailure>>();
            Capture(
                request,
                (wrapper, req, result) =>
                    taskSource.TrySetResult(
                        new CallbackResult<TotalCaptureResult, CaptureFailure> { Successful = true, Result = result }),
                (wrapper, req, failure) => 
                    taskSource.TrySetResult(
                        new CallbackResult<TotalCaptureResult, CaptureFailure> { Successful = false, Error = failure }),
                handler);
            return taskSource.Task;
        }
    }
}