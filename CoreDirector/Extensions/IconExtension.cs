using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CoreDirector.Interop;

namespace CoreDirector.Extensions
{
    internal static class IconUtilities
    {
        public static ImageSource ToImageSource(this Icon icon)
        {
            var bitmap = icon.ToBitmap();
            var hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!NativeMethods.DeleteObject(hBitmap))
                throw new Win32Exception();

            return wpfBitmap;
        }
    }
}
