using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using CoreDirector.Interop;
using ModernWpf;

namespace CoreDirector
{
    public partial class MainWindow
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Deactivated += OnDeactivated;

            Hide();
        }
        #endregion

        #region Window Events
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Top = SystemParameters.WorkArea.Bottom - Height - 10;
            Left = SystemParameters.WorkArea.Right - Width - 10;

            var presentationSource = PresentationSource.FromVisual((Visual)sender);

            if (presentationSource is not null)
                presentationSource.ContentRendered += PresentationSourceOnContentRendered;
        }

        private void PresentationSourceOnContentRendered(object? sender, EventArgs e)
        {
            if (sender is not HwndSource hwndSource)
                return;

            UpdateStyleAttributes(hwndSource);
            ThemeManager.Current.ActualApplicationThemeChanged += delegate { UpdateStyleAttributes(hwndSource); };
        }

        private void TaskbarIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            Show();
            Activate();
        }

        private void OnDeactivated(object? sender, EventArgs e)
        {
            Hide();
        }
        #endregion

        #region Private Methods
        private void EnableMica(HwndSource source, bool darkThemeEnabled)
        {
            int trueValue = 0x01;
            int falseValue = 0x00;

            if (darkThemeEnabled)
            {
                NativeMethods.DwmSetWindowAttribute(source.Handle, NativeMethods.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, Marshal.SizeOf(typeof(int)));
            }
            else
            {
                NativeMethods.DwmSetWindowAttribute(source.Handle, NativeMethods.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref falseValue, Marshal.SizeOf(typeof(int)));
            }

            NativeMethods.DwmSetWindowAttribute(source.Handle, NativeMethods.DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
        }

        private void UpdateStyleAttributes(HwndSource hwnd)
        {
            EnableMica(hwnd, ThemeManager.Current.ActualApplicationTheme is ApplicationTheme.Dark);
        }
        #endregion
    }
}
