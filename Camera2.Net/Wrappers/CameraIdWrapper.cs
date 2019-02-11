using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Camera2.Net.Wrappers;
using Java.Lang;

namespace Camera2.Net.Wrappers
{
    public class CameraIdWrapper
    {
        public string CameraId { get; }
        public LensFacing LensFacing { get; }
        public StreamConfigurationMapWrapper StreamConfigurationMap { get; }
        public int Order { get; }

        public CameraIdWrapper(string cameraId, CameraManagerWrapper managerWrapper)
        {
            CameraId = cameraId;
            var characteristics = managerWrapper.GetCameraCharacteristics(cameraId);
            LensFacing = GetLensFacing(characteristics);
            StreamConfigurationMap = GetStreamConfigurationMap(characteristics);
            switch (LensFacing)
            {
                case LensFacing.Back:
                    Order = 0;
                    break;
                case LensFacing.External:
                    Order = 1;
                    break;
                case LensFacing.Front:
                    Order = 2;
                    break;
                default:
                    Order = 3;
                    break;
            }
        }

        private static StreamConfigurationMapWrapper GetStreamConfigurationMap(CameraCharacteristics characteristics)
        {
            return new StreamConfigurationMapWrapper((StreamConfigurationMap) characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap));
        }

        private static LensFacing GetLensFacing(CameraCharacteristics characteristics)
        {
            return (LensFacing) ((Integer) characteristics.Get(CameraCharacteristics.LensFacing)).IntValue();
        }
    }
}