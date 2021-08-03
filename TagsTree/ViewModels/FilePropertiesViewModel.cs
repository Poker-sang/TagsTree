using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using TagsTree.Annotations;
using TagsTree.Commands;
using TagsTree.Models;
using TagsTree.Services;

namespace TagsTree.ViewModels
{
	public class FilePropertiesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void Load(FileModel file)
		{
			if (file.IsFolder)
			{
				Extension = "文件夹";
				Name = file.Name;
			}
			else
			{
				Extension = file.Name.Split('.', StringSplitOptions.RemoveEmptyEntries).Last().ToUpper();
				Name = file.Name[..(file.Name.Length - _extension.Length - 1)];
			}
			Path = file.PartialPath;
			Icon = App.CIconOfPath.IconOfPathLarge(file.FullName, true, true);
			var tags = App.Relations.GetTags(file).Aggregate("", (current, tag) => current + " " + tag);
			Tags = tags is "" ? "" : tags[1..];
		}

		public FilePropertiesViewModel()
		{
			static bool Func1(object? _) => true;
			bool Func2(object? _) => _exists;
			_openBClick = new RelayCommand(Func2, FilePropertiesService.OpenBClick);
			_openExplorerBClick = new RelayCommand(Func1, FilePropertiesService.OpenExplorerBClick);
			_editTagsBClick = new RelayCommand(Func1, FilePropertiesService.EditTagsBClick);
			_removeBClick = new RelayCommand(Func1, FilePropertiesService.RemoveBClick);
			_renameBClick = new RelayCommand(Func2, FilePropertiesService.RenameBClick);
			_moveBClick = new RelayCommand(Func2, FilePropertiesService.MoveBClick);
			_deleteBClick = new RelayCommand(Func2, FilePropertiesService.DeleteBClick);
		}
		private RelayCommand _openBClick;
		private RelayCommand _openExplorerBClick;
		private RelayCommand _editTagsBClick;
		private RelayCommand _removeBClick;
		private RelayCommand _renameBClick;
		private RelayCommand _moveBClick;
		private RelayCommand _deleteBClick;

		private bool _exists;

		private ImageSource _icon;
		private string _name = "";
		private string _path = "";
		private string _extension = "";
		private string _tags = "";


		public RelayCommand OpenBClick
		{
			get => _openBClick;
			set
			{
				if (Equals(_openBClick, value)) return;
				_openBClick = value;
				OnPropertyChanged(nameof(OpenBClick));
			}
		}
		public RelayCommand OpenExplorerBClick
		{
			get => _openExplorerBClick;
			set
			{
				if (Equals(_openExplorerBClick, value)) return;
				_openExplorerBClick = value;
				OnPropertyChanged(nameof(OpenExplorerBClick));
			}
		}
		public RelayCommand EditTagsBClick
		{
			get => _editTagsBClick;
			set
			{
				if (Equals(_editTagsBClick, value)) return;
				_editTagsBClick = value;
				OnPropertyChanged(nameof(EditTagsBClick));
			}
		}
		public RelayCommand RemoveBClick
		{
			get => _removeBClick;
			set
			{
				if (Equals(_removeBClick, value)) return;
				_removeBClick = value;
				OnPropertyChanged(nameof(RemoveBClick));
			}
		}
		public RelayCommand RenameBClick
		{
			get => _renameBClick;
			set
			{
				if (Equals(_renameBClick, value)) return;
				_renameBClick = value;
				OnPropertyChanged(nameof(RenameBClick));
			}
		}
		public RelayCommand MoveBClick
		{
			get => _moveBClick;
			set
			{
				if (Equals(_moveBClick, value)) return;
				_moveBClick = value;
				OnPropertyChanged(nameof(MoveBClick));
			}
		}
		public RelayCommand DeleteBClick
		{
			get => _deleteBClick;
			set
			{
				if (Equals(_deleteBClick, value)) return;
				_deleteBClick = value;
				OnPropertyChanged(nameof(DeleteBClick));
			}
		}

		public bool Exists
		{
			get => _exists;
			set
			{
				if (Equals(_exists, value)) return;
				_exists = value;
				OnPropertyChanged(nameof(Exists));
				_openExplorerBClick.OnCanExecuteChanged();
				_renameBClick.OnCanExecuteChanged();
				_moveBClick.OnCanExecuteChanged();
				_deleteBClick.OnCanExecuteChanged();
			}
		}
		public ImageSource Icon
		{
			get => _icon;
			set
			{
				if (Equals(_icon, value)) return;
				_icon = value;
				OnPropertyChanged(nameof(Icon));
			}
		}
		public string Name
		{
			get => _name;
			set
			{
				if (Equals(_name, value)) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
		public string Path
		{
			get => _path;
			set
			{
				if (Equals(_path, value)) return;
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}
		public string Extension
		{
			get => _extension;
			set
			{
				if (Equals(_extension, value)) return;
				_extension = value;
				OnPropertyChanged(nameof(Extension));
			}
		}
		public string Tags
		{
			get => _tags;
			set
			{
				if (Equals(_tags, value)) return;
				_tags = value;
				OnPropertyChanged(nameof(Tags));
			}
		}
	}
}
