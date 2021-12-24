using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using CoreDirector.Annotations;

namespace CoreDirector.Models
{
    internal class AppProcess : INotifyPropertyChanged
    {
        #region Properties
        public string Name { get; init; }

        public Image Icon { get; init; }

        public CoreType Type { get; set; } = CoreType.Default;
        #endregion

        #region Constructor
        public AppProcess(Image icon, string name)
        {
            Icon = icon;
            Name = name;
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
