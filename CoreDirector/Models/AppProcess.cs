using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoreDirector.Managers;
using CoreDirector.Supports;
using CoreDirector.Utilities;
using Newtonsoft.Json;

namespace CoreDirector.Models
{
    internal class AppProcess : INotifyPropertyChanged
    {
        #region Properties
        [JsonIgnore]
        public string Key => CreateKey(FilePath);

        [JsonIgnore]
        public string Name => Path.GetFileName(FilePath);

        [JsonProperty("filePath")]
        public string FilePath { get; }

        [JsonIgnore]
        public Bitmap? IconBitmap { get; set; }

        [JsonIgnore]
        public ConcurrentBag<Process> Items { get; } = new();

        [JsonProperty("coreType")]
        public CoreType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        private CoreType _type;
        #endregion

        #region Constructor
        public AppProcess(string filePath, Bitmap? iconBitmap, IEnumerable<Process>? items)
        {
            FilePath = filePath;
            IconBitmap = iconBitmap;

            if (items is not null)
            {
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
        }
        #endregion

        #region Public Methods
        public void ApplyAffinity()
        {
            Task.Run(() =>
            {
                try
                {
                    RefreshProcesses();

                    foreach (var item in Items)
                    {
                        ProcessorUtility.SetAffinity(item, Type);
                    }

                    var cachePath = Path.Combine(EnvironmentSupport.Cache, Key);

                    if (Type is CoreType.Default)
                    {
                        ConfigManager.Config.SavedProcesses.Remove(Key);
                        File.Delete(cachePath);
                    }
                    else
                    {
                        ConfigManager.Config.SavedProcesses[Key] = this;

                        if (!File.Exists(cachePath))
                            IconBitmap?.Save(cachePath, ImageFormat.Icon);
                    }

                    ConfigManager.Save();
                }
                catch
                {
                    // ignored
                }
            });
        }

        public void RefreshProcesses()
        {
            Items.Clear();
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(FilePath));

            foreach (var process in processes)
            {
                Items.Add(process);
            }
        }

        public static string CreateKey(string input)
        {
            using var md5 = MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();

            foreach (var value in hashBytes)
            {
                sb.Append(value.ToString("X2"));
            }

            return sb.ToString();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(Type))
                ApplyAffinity();
        }
        #endregion
    }
}
