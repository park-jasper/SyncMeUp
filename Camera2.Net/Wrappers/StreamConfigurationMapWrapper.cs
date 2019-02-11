using Android.Graphics;
using Android.Hardware.Camera2.Params;
using Android.Util;

namespace Camera2.Net.Wrappers
{
    public class StreamConfigurationMapWrapper
    {
        private readonly StreamConfigurationMap _internalMap;
        public bool HasConfigurationMap => _internalMap != null;

        internal StreamConfigurationMapWrapper(StreamConfigurationMap internalMap)
        {
            _internalMap = internalMap;
        }

        public Size[] GetOutputSizes(ImageFormatType imageFormat)
        {
            return _internalMap.GetOutputSizes((int) imageFormat);
        }
    }
}