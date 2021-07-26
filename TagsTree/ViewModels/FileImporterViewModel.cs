using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TagsTree.Annotations;
using TagsTree.Commands;
using TagsTree.Models;
using TagsTree.Services;

namespace TagsTree.ViewModels
{
	public sealed class FileImporterViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FileImporterViewModel()
		{
			bool Func1(object? _) => !Importing;
			bool Func2(object? _) => !Importing && _fileModels.Count != 0;
			_import = new RelayCommand(Func1, FileImporterService.Import);
			_deleteBClick = new RelayCommand(Func2, FileImporterService.DeleteBClick);
			_saveBClick = new RelayCommand(Func2, FileImporterService.SaveBClick);

			_fileModels.CollectionChanged += (_, _) =>
			{
				_deleteBClick.OnCanExecuteChanged();
				_saveBClick.OnCanExecuteChanged();
			};
		}

		private bool _importing;
		private ObservableCollection<FileModel> _fileModels = new();
		private RelayCommand _import;
		private RelayCommand _deleteBClick;
		private RelayCommand _saveBClick;

		public bool Importing
		{ 
			get => _importing;
			set
			{
				if (Equals(_importing, value)) return;
				_importing = value;
				_import.OnCanExecuteChanged();
				_deleteBClick.OnCanExecuteChanged();
				_saveBClick.OnCanExecuteChanged();
			}
		}

		public ObservableCollection<FileModel> FileModels
		{
			get => _fileModels;
			set
			{
				if (Equals(_fileModels, value)) return;
				_fileModels = value;
				OnPropertyChanged(nameof(FileModels));
			}
		}

		public RelayCommand Import
		{
			get => _import;
			set
			{
				if (Equals(_import, value)) return;
				_import = value;
				OnPropertyChanged(nameof(Import));
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
		public RelayCommand SaveBClick
		{
			get => _saveBClick;
			set
			{
				if (Equals(_saveBClick, value)) return;
				_saveBClick = value;
				OnPropertyChanged(nameof(SaveBClick));
			}
		}
	}
}
