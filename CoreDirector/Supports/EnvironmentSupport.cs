using System;
using System.IO;

namespace CoreDirector.Supports
{
    internal class EnvironmentSupport
    {
        #region Fields
        private static readonly string _storage = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\CoreDirector";
        #endregion

        #region Constants
        private const string cacheDir = "Cache";
        private const string config = "Config.json";
        #endregion

        #region Properties
        public static string Cache
        {
            get
            {
                var combine = Path.Combine(Storage, cacheDir);

                if (!Directory.Exists(combine))
                    Directory.CreateDirectory(combine);

                return combine;
            }
        }

        public static string Storage
        {
            get
            {
                if (!Directory.Exists(_storage))
                    Directory.CreateDirectory(_storage);

                return _storage;
            }
        }

        public static string Config => Path.Combine(Storage, config);
        #endregion
    }
}
