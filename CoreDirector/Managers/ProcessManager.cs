using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using CoreDirector.Extensions;
using CoreDirector.Models;

namespace CoreDirector.Managers
{
    internal class ProcessManager
    {
        public static IEnumerable<AppProcess> GetAppProcesses()
        {
            Process[] processes = Process.GetProcesses();

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

                var iconBitmap = !string.IsNullOrEmpty(filePath)
                    ? Icon.ExtractAssociatedIcon(filePath)?.ToBitmap()
                    : default;

                yield return new AppProcess(filePath, iconBitmap, processGroup);
            }
        }
    }
}
