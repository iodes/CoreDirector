using System;
using System.Management;
using CoreDirector.Managers;
using CoreDirector.Models;

namespace CoreDirector.Commons
{
    internal class ProcessWatcher : IDisposable
    {
        #region Fields
        private static ManagementEventWatcher? _startWatcher;
        #endregion

        #region Public Methods
        public void Start()
        {
            _startWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'"));

            _startWatcher.EventArrived += (_, args) =>
            {
                var targetInstance = (ManagementBaseObject)args.NewEvent["TargetInstance"];
                var executablePath = targetInstance["ExecutablePath"]?.ToString();

                if (string.IsNullOrEmpty(executablePath))
                    return;

                var processKey = AppProcess.CreateKey(executablePath);

                if (ConfigManager.Config.SavedProcesses.TryGetValue(processKey, out var appProcess))
                    appProcess.ApplyAffinity();
            };

            _startWatcher.Start();
        }

        public void Stop()
        {
            _startWatcher?.Stop();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Stop();
            _startWatcher?.Dispose();
        }
        #endregion
    }
}
