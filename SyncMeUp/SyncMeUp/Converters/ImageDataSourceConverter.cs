using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace SyncMeUp.Converters
{
    public class ImageDataSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] imageData)
            {
                return ImageSource.FromStream(() => new MemoryStream(imageData));
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}