using CoreDirector.Models;
using System;
using System.Diagnostics;
using System.Management;

namespace CoreDirector.Utilities
{
    internal static class ProcessorUtility
    {
        public static void SetAffinity(int processId, CoreType type)
        {
            var processor = GetProcessor();
            using var process = Process.GetProcessById(processId);

            long defaultAffinity = (1 << processor.ThreadCount) - 1;
            long performanceAffinity = (1 << processor.ThreadCount - processor.EfficientCoreCount) - 1;

            switch (type)
            {
                case CoreType.Default:
                    process.ProcessorAffinity = new IntPtr(defaultAffinity);
                    break;

                case CoreType.Efficient:
                    process.ProcessorAffinity = new IntPtr(defaultAffinity ^ performanceAffinity);
                    break;

                case CoreType.Performance:
                    process.ProcessorAffinity = new IntPtr(performanceAffinity);
                    break;
            }
        }

        public static Processor GetProcessor()
        {
            var threadCount = Environment.ProcessorCount;

            using var objectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            using var objectCollection = objectSearcher.Get();

            int coreCount = 0;
            string? processorName = null;

            foreach (var item in objectCollection)
            {
                if (string.IsNullOrEmpty(processorName))
                    processorName = item["Name"].ToString()!;

                coreCount += int.Parse(item["NumberOfCores"].ToString()!);
            }

            return new Processor(processorName!)
            {
                ThreadCount = threadCount,
                TotalCoreCount = coreCount
            };
        }
    }
}
