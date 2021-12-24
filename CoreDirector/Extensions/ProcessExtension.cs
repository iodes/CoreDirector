using System.Diagnostics;

namespace CoreDirector.Extensions
{
    internal static class ProcessExtension
    {
        public static string GetSafeFileName(this Process process)
        {
            try
            {
                return process.MainModule?.FileName ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
