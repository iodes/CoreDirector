using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoreDirector.Models
{
    internal class AppConfig
    {
        [JsonProperty("savedProcesses")]
        public Dictionary<string, AppProcess> SavedProcesses { get; set; } = new();
    }
}
