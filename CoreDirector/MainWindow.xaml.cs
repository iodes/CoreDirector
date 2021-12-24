using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using CoreDirector.Commons;
using CoreDirector.Interop;
using CoreDirector.Managers;
using CoreDirector.Models;
using ModernWpf;

namespace CoreDirector
{
    public partial class MainWindow
    {
        #region Fields
        private readonly object _lock = new();

        private readonly ProcessWatcher _processWatcher = new();

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
            _processWatcher.Start();

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

            Task.Run(UpdateProcesses);
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

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collectionView = CollectionViewSource.GetDefaultView(processListView.ItemsSource) as CollectionView;

            if (collectionView is null)
                return;

            switch (tabControl.SelectedIndex)
            {
                case 0:
                    collectionView.Filter = null;
                    break;

                case 1:
                    collectionView.Filter = o => FilterListView(o, CoreType.Performance);
                    break;

                case 2:
                    collectionView.Filter = o => FilterListView(o, CoreType.Efficient);
                    break;
            }
        }

        private void TaskbarIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            Show();
            Activate();
        }
        #endregion

        #region Private Methods
        private void UpdateProcesses()
        {
            _processes.Clear();

            IOrderedEnumerable<AppProcess> appProcesses = ProcessManager.GetAppProcesses()
                .OrderBy(x => x.Name);

            foreach (var appProcess in appProcesses)
            {
                _processes.Add(appProcess);
            }
        }

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

        private bool FilterListView(object item, CoreType type)
        {
            if (item is not AppProcess appProcess)
                return false;

            return appProcess.Type == type;
        }
        #endregion
    }
}
