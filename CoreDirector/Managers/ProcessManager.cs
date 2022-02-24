using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using CoreDirector.Extensions;
using CoreDirector.Models;
using CoreDirector.Supports;

namespace CoreDirector.Managers
{
    internal class ProcessManager
    {
        public static IEnumerable<AppProcess> GetAppProcesses()
        {
            foreach (var (key, appProcess) in ConfigManager.Config.SavedProcesses)
            {
                appProcess.IconBitmap = new Bitmap(Path.Combine(EnvironmentSupport.Cache, key));
                appProcess.ApplyAffinity();

                yield return appProcess;
            }

            IEnumerable<IGrouping<string, Process>> processGroups = Process.GetProcesses()
                .GroupBy(x => x.ProcessName);

            foreach (IGrouping<string, Process> processGroup in processGroups)
            {
                var filePath = processGroup.FirstOrDefault()?.GetSafeFileName();

                if (string.IsNullOrEmpty(filePath))
                    continue;

                var processKey = AppProcess.CreateKey(filePath);

                if (ConfigManager.Config.SavedProcesses.ContainsKey(processKey))
                    continue;

                Bitmap? iconBitmap = default;

                try
                {
                    iconBitmap = Icon.ExtractAssociatedIcon(filePath)?.ToBitmap();
                }
                catch
                {
                    // ignored
                }

                yield return new AppProcess(filePath, iconBitmap, processGroup);
            }
        }
    }
}
