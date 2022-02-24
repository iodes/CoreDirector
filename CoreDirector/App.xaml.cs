using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using CoreDirector.Supports;
using Serilog;

namespace CoreDirector
{
    public partial class App
    {
        // ReSharper disable once NotAccessedField.Local
        private Mutex? _mutex;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(EnvironmentSupport.Logs, "log-.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _mutex = new Mutex(true, $"{nameof(CoreDirector)}.App.Mutex", out bool createdNew);

            if (!createdNew)
            {
                Log.Warning("The app is already running. Terminates the current process.");
                Current.Shutdown();

                return;
            }

            Log.Information($"CoreDirector {Assembly.GetExecutingAssembly().GetName().Version} started successfully.");
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is not Exception exception)
                return;

            Log.Error(exception, "An unknown error has occurred.");
            MessageBox.Show(exception.Message, "프로그램 오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
