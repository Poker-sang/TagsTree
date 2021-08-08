using JetBrains.Annotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels
{
	public class FileViewModel : FileModel, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FileViewModel(FileModel fileModel) : base(fileModel) { }

		public FileViewModel(FileModel fileModel, TagModel tag) : base(fileModel)
		{
			_selected = HasTag(tag);
			_virtualTags = Tags;
		}

		public FileViewModel(string fullName, bool isFolder) : base(fullName, isFolder) { }

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
		}

		public new static bool ValidPath(string path) => FileModel.ValidPath(path);
		public TagModel? GetRelativeVirtualTag(TagModel parentTag) => VirtualTags.GetTagModels().FirstOrDefault(parentTag.HasChildTag);
		public void TagsUpdated()
		{
			OnPropertyChanged(nameof(Tags));
			OnPropertyChanged(nameof(VirtualTags));
		}

		private bool? _selected;
		private string _virtualTags = "";

		public bool? Selected
		{
			get => _selected;
			set
			{
				if (Equals(_selected, value)) return;
				_selected = value;
				OnPropertyChanged(nameof(Selected));
			}
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