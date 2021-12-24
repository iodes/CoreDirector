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
    internal static class BitmapExtension
    {
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();

            var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            if (!NativeMethods.DeleteObject(hBitmap))
                throw new Win32Exception();

            return wpfBitmap;
        }
    }
}
