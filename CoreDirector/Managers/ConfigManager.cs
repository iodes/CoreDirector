using System.IO;
using CoreDirector.Models;
using CoreDirector.Supports;
using Newtonsoft.Json;

namespace CoreDirector.Managers
{
    internal static class ConfigManager
    {
        #region Properties
        public static AppConfig Config
        {
            get
            {
                if (_config is not null)
                    return _config;

                if (!File.Exists(EnvironmentSupport.Config))
                {
                    _config = new AppConfig();
                    Save();
                }

                var json = File.ReadAllText(EnvironmentSupport.Config);
                _config = JsonConvert.DeserializeObject<AppConfig>(json)!;

                return _config;
            }
        }

        private static AppConfig? _config;
        #endregion

        #region Public Methods
        public static void Save()
        {
            var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(EnvironmentSupport.Config, json);
        }
        #endregion
    }
}
