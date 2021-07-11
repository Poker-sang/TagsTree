using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TagsTree.Annotations;
using TagsTree.Services;
using TagsTree.Commands;
using TagsTree.Models;

namespace TagsTree.ViewModels
{
	public sealed class FileImporterViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FileImporterViewModel()
		{
			static bool Func1(object? _) => true;
			bool Func2(object? _) => _fileModels.Count != 0;
			_import = new RelayCommand(Func1, FileImporterService.Import);
			_deleteBClick = new RelayCommand(Func2, FileImporterService.DeleteBClick);
			_saveBClick = new RelayCommand(Func2, FileImporterService.SaveBClick);
			_fileModels.CollectionChanged += (_, _) =>
			{
				_deleteBClick.OnCanExecuteChanged();
				_saveBClick.OnCanExecuteChanged();
			};
		}

		private ObservableCollection<FileModel> _fileModels = new();
		private RelayCommand _import;
		private RelayCommand _deleteBClick;
		private RelayCommand _saveBClick;

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
