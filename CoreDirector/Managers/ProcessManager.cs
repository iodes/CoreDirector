using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CoreDirector.Extensions;
using CoreDirector.Models;
using CoreDirector.Supports;
using CoreDirector.Utilities;

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

                var iconBitmap = !string.IsNullOrEmpty(filePath)
                    ? Icon.ExtractAssociatedIcon(filePath)?.ToBitmap()
                    : default;

                yield return new AppProcess(filePath, iconBitmap, processGroup);
            }
        }
    }
}
