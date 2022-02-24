using System;
using System.Threading;
using System.Windows;

namespace CoreDirector
{
    public partial class App
    {
        // ReSharper disable once NotAccessedField.Local
        private Mutex? _mutex;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            _mutex = new Mutex(true, $"{nameof(CoreDirector)}.App.Mutex", out bool createdNew);

            if (!createdNew)
                Current.Shutdown();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                MessageBox.Show(exception.Message, "프로그램 오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
