using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.ViewModels
{
	public class FileViewModel : FileModel, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FileViewModel(FileModel fileModel) : base(fileModel)
		{
			_fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
			GetIcon();
		}

		public FileViewModel(FileModel fileModel, TagViewModel tag) : base(fileModel)
		{
			_fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
			Selected = SelectedOriginal = HasTag(tag);
			GetIcon();
		}

		public FileViewModel(string fullName) : base(fullName)
		{
			_fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
			GetIcon();
		}

		private void GetIcon()
		{
			if (Exists)
				Icon = IsFolder ? IconX.FolderIcon : IconX.NotFoundIcon; // await IconX.GetIcon(Extension);
			else Icon = IconX.NotFoundIcon;
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
			GetIcon();
			OnPropertyChanged(nameof(Icon));
			OnPropertyChanged(nameof(DateOfModification));
			OnPropertyChanged(nameof(Size));
		}

		private readonly FileSystemInfo _fileSystemInfo;

		public BitmapImage Icon { get; private set; } = null!;

		public string DateOfModification => Exists ? _fileSystemInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture) : "";
		public string Size => Exists && !IsFolder ? FileX.CountSize((FileInfo)_fileSystemInfo) : "";
		public bool Exists => _fileSystemInfo.Exists;

		public new static bool ValidPath(string path) => FileModel.ValidPath(path);
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