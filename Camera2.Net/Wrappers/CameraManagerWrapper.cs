using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Hardware.Camera2;
using Android.OS;
using Camera2.Net.Callback;

namespace Camera2.Net.Wrappers
{
    public class CameraManagerWrapper
    {
        public static CameraManagerWrapper GetCameraManager(Context context)
        {
            var internalManager = (CameraManager) context.GetSystemService(Context.CameraService);
            return new CameraManagerWrapper(internalManager);
        }

        private readonly CameraManager _internalManager;
        private CameraManagerWrapper(CameraManager internalManager)
        {
            _internalManager = internalManager;
        }

        public CameraCharacteristics GetCameraCharacteristics(string cameraIdString)
        {
            return _internalManager.GetCameraCharacteristics(cameraIdString);
        }

        public CameraCharacteristics GetCameraCharacteristics(CameraIdWrapper cameraIdWrapper) => GetCameraCharacteristics(cameraIdWrapper.CameraId);

        public string[] GetCameraIdList()
        {
            return _internalManager.GetCameraIdList();
        }
        public CameraIdWrapper[] GetCameraIds()
        {
            return GetCameraIdList().Select(id => new CameraIdWrapper(id, this)).ToArray();
        }

        public void OpenCamera(CameraIdWrapper cameraIdWrapper, Action<CameraDeviceWrapper> onOpened, Action<CameraDeviceWrapper, CameraError> onError, Action<CameraDeviceWrapper> onDisconnected, Handler handler)
        {
            _internalManager.OpenCamera(
                cameraIdWrapper.CameraId,
                new CameraDeviceStateCallback(onOpened, onError, onDisconnected),
                handler);
        }

        public Task<CallbackResult<CameraDeviceWrapper, CameraError>> OpenCameraAsync(CameraIdWrapper cameraIdWrapper, Action<CameraDeviceWrapper> onDisconnected, Handler handler)
        {
            var taskSource = new TaskCompletionSource<CallbackResult<CameraDeviceWrapper, CameraError>>();
            OpenCamera(
                cameraIdWrapper,
                device => taskSource.TrySetResult(
                    new CallbackResult<CameraDeviceWrapper, CameraError>
                    {
                        Successful = true,
                        Result = device
                    }),
                (device, error) => taskSource.TrySetResult(
                    new CallbackResult<CameraDeviceWrapper, CameraError>
                    {
                        Successful = false,
                        Result = device,
                        Error = error
                    }),
                onDisconnected,
                handler);
            return taskSource.Task;
        }
    }
}