using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TagsTree.Models;

namespace TagsTree.ViewModels
{
	public class FileViewModel : FileModel, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FileViewModel(FileModel fileModel) : base(fileModel) => _virtualTags = Tags;

		public FileViewModel(FileModel fileModel, string tag) : base(fileModel)
		{
			_selected = App.Relations[GetFileModel, tag];
			_virtualTags = Tags;
		}

		public FileViewModel(string fullName, bool isFolder) : base(fullName, isFolder) => _virtualTags = Tags;

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
		
		private bool _selected;
		private string _virtualTags;

		public bool Selected
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