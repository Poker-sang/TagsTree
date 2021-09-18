using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels
{
    public class FileViewModel : FileModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public FileViewModel(FileModel fileModel) : base(fileModel)
        {
            _fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
        }

        public FileViewModel(FileModel fileModel, TagViewModel tag) : base(fileModel)
        {
            _fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
            Selected = SelectedOriginal = HasTag(tag);
        }

        public FileViewModel(string fullName) : base(fullName)
        {
            _fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
        }

        public FileModel GetFileModel() => App.IdFile[Id];
        public FileModel NewFileModel() => new(this);

        public new void Reload(string fullName)
        {
            base.Reload(fullName);
            GetFileModel().Reload(fullName); //更新UI
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Extension));
            OnPropertyChanged(nameof(Path));
            OnPropertyChanged(nameof(PartialPath));
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(DateOfModification));
            OnPropertyChanged(nameof(Size));
        }

        private readonly FileSystemInfo _fileSystemInfo;

        public void IconChange() => OnPropertyChanged(nameof(Icon));
        private string ansdah => Name;//debug

        public BitmapImage Icon => this.GetIcon();

        public string DateOfModification => Exists ? _fileSystemInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture) : "";
        public string Size => Exists && !IsFolder ? FileSystemHelper.CountSize((FileInfo)_fileSystemInfo) : "";
        public bool Exists => _fileSystemInfo.Exists;

        public new static bool IsValidPath(string path) => FileModel.IsValidPath(path);
        public void TagsUpdated() => OnPropertyChanged(nameof(Tags));

        public bool? Selected { get; private set; }
        public bool? SelectedOriginal { get; }
        public new string Tags => base.Tags;
        public new string PartialPath => base.PartialPath;
        public void SelectedFlip()
        {
            Selected = Selected == SelectedOriginal ?
                Selected switch
                {
                    true => false,
                    false => true,
                    null => false
                }
                : SelectedOriginal;
            OnPropertyChanged(nameof(Selected));
        }
    }
}