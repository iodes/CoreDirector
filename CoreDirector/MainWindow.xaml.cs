using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using CoreDirector.Extensions;
using CoreDirector.Interop;
using CoreDirector.Models;
using ModernWpf;

namespace CoreDirector
{
    public partial class MainWindow
    {
        #region Fields
        private readonly object _lock = new();

        private ObservableCollection<AppProcess> _processes { get; } = new();
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            processListView.ItemsSource = _processes;

            Loaded += OnLoaded;
            Activated += OnActivated;
            Deactivated += OnDeactivated;

            BindingOperations.EnableCollectionSynchronization(_processes, _lock);

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

        private void OnActivated(object? sender, EventArgs e)
        {
            Task.Run(UpdateProcesses);
        }

        private void OnDeactivated(object? sender, EventArgs e)
        {
            Hide();
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

        private void UpdateProcesses()
        {
            _processes.Clear();
            Process[] processes = Process.GetProcesses();

            try
            {
                IEnumerable<IGrouping<string, Process>> processGroups = processes
                    .OrderBy(x => x.ProcessName)
                    .GroupBy(x => x.ProcessName);

                foreach (IGrouping<string, Process> processGroup in processGroups)
                {
                    var process = processGroup.FirstOrDefault();

                    if (process is null)
                        continue;

                    var filePath = process.GetSafeFileName();

                    if (string.IsNullOrEmpty(filePath))
                        continue;

                    ImageSource? icon = null;

                    Dispatcher.Invoke(() =>
                    {
                        icon = !string.IsNullOrEmpty(filePath)
                            ? System.Drawing.Icon.ExtractAssociatedIcon(filePath)?.ToImageSource()
                            : default;
                    });

                    _processes.Add(new AppProcess(process.Id, filePath, icon!));
                }
            }
            finally
            {
                foreach (var process in processes)
                {
                    process.Dispose();
                }
            }
        }
    }
}
