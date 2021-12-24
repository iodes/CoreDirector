using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using CoreDirector.Annotations;
using CoreDirector.Utilities;

namespace CoreDirector.Models
{
    internal class AppProcess : INotifyPropertyChanged
    {
        #region Properties
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private int _id;

        public string Name => Path.GetFileName(FilePath);

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _filePath;

        public ImageSource Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _icon;

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
        public AppProcess(int id, string filePath, ImageSource icon)
        {
            _id = id;
            _filePath = filePath;
            _icon = icon;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(Type))
                Task.Run(() => ProcessorUtility.SetAffinity(Id, Type));
        }
        #endregion
    }
}
