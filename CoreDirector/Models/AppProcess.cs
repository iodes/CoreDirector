using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CoreDirector.Utilities;

namespace CoreDirector.Models
{
    internal class AppProcess : INotifyPropertyChanged
    {
        #region Properties
        public string Name => Path.GetFileName(FilePath);

        public string FilePath { get; }

        public Bitmap? IconBitmap { get; }

        public IEnumerable<Process> Items { get; }

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
        public AppProcess(string filePath, Bitmap? iconBitmap, IEnumerable<Process> items)
        {
            FilePath = filePath;
            IconBitmap = iconBitmap;
            Items = items;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(Type))
            {
                Task.Run(() =>
                {
                    try
                    {
                        foreach (var item in Items)
                        {
                            ProcessorUtility.SetAffinity(item, Type);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                });
            }
        }
        #endregion
    }
}
