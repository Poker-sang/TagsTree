using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
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
			_fileSystemInfo = fileModel.IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
		}

		public FileViewModel(FileModel fileModel, TagModel tag) : base(fileModel)
		{
			Selected = SelectedOriginal = HasTag(tag);
			_fileSystemInfo = IsFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
		}

		public FileViewModel(string fullName, bool isFolder) : base(fullName, isFolder)
		{
			_fileSystemInfo = isFolder ? new DirectoryInfo(FullName) : new FileInfo(FullName);
		}

		public FileModel GetFileModel => App.IdFile[Id];
		public FileModel NewFileModel() => new(this);

		public new void Reload(string fullName)
		{
			base.Reload(fullName);
			GetFileModel.Reload(fullName);
			App.SaveFiles();
			OnPropertyChanged(nameof(Name));
			OnPropertyChanged(nameof(Extension));
			OnPropertyChanged(nameof(Path));
			OnPropertyChanged(nameof(PartialPath));
			OnPropertyChanged(nameof(Icon));
			OnPropertyChanged(nameof(DateOfModification));
			OnPropertyChanged(nameof(Size));
		}

		private readonly FileSystemInfo _fileSystemInfo;
		public ImageSource Icon
		{
			get
			{
				//if (Exists)
				//{
				//	if (IsFolder)
				//		return Services.BitmapX.FolderIcon;
				//	if (System.Drawing.Icon.ExtractAssociatedIcon(FullName) is { } icon)
				//		return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				//}
				//return Services.BitmapX.NotFoundIcon;
				return null!;
			}
		}
		public string DateOfModification => Exists ? _fileSystemInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture) : "";
		public string Size => Exists && !IsFolder ? FileX.CountSize((FileInfo)_fileSystemInfo) : "";
		public bool Exists => _fileSystemInfo.Exists;

		public new static bool ValidPath(string path) => FileModel.ValidPath(path);
		public TagModel? GetRelativeVirtualTag(TagModel parentTag) => VirtualTags.GetTagModels().FirstOrDefault(parentTag.HasChildTag);
		public void TagsUpdated()
		{
			OnPropertyChanged(nameof(Tags));
			OnPropertyChanged(nameof(VirtualTags));
		}
		public void VirtualTagsInitialize() => VirtualTags = Tags;

		public bool? Selected { get; private set; }
		public bool? SelectedOriginal { get; }
		public new string Tags => base.Tags;
		public new string PartialPath => base.PartialPath;
		private string _virtualTags = "";

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
		public string VirtualTags
		{
			get => _virtualTags;
			set
			{
				if (Equals(_virtualTags, value)) return;
				_virtualTags = value;
				OnPropertyChanged(nameof(VirtualTags));
			}
		}
	}
}