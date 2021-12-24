using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using CoreDirector.Annotations;

namespace CoreDirector.Models
{
    internal class AppProcess : INotifyPropertyChanged
    {
        #region Properties
        public string Name { get; init; }

        public string FilePath { get; init; }

        public ImageSource Icon { get; init; }

        public CoreType Type { get; set; } = CoreType.Default;
        #endregion

        #region Constructor
        public AppProcess(string filePath, ImageSource icon)
        {
            FilePath = filePath;
            Name = Path.GetFileName(filePath);
            Icon = icon;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
