using System;
using Android.Media;

namespace Camera2.Net.Callback
{
    public class ImageReaderOnImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private readonly Action<ImageReader> _onImageAvailable;

        public ImageReaderOnImageAvailableListener(Action<ImageReader> onImageAvailable)
        {
            _onImageAvailable = onImageAvailable;
        }
        public void OnImageAvailable(ImageReader reader)
        {
            _onImageAvailable?.Invoke(reader);
        }
    }
}