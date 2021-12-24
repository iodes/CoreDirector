using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using CoreDirector.Extensions;

namespace CoreDirector.Markups
{
    internal class BitmapToImageSourceConverter : ValueConverterBase<Bitmap, ImageSource?>
    {
        public override ImageSource? Convert(Bitmap? value, object parameter, CultureInfo culture)
        {
            return value?.ToImageSource();
        }

        public override Bitmap ConvertBack(ImageSource? value, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
