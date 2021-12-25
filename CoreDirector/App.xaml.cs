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
            _mutex = new Mutex(true, $"{nameof(CoreDirector)}.App.Mutex", out bool createdNew);

            if (!createdNew)
                Current.Shutdown();
        }
    }
}
