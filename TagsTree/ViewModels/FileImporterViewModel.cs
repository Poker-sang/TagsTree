using JetBrains.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TagsTree.Commands;
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
			Import = new RelayCommand(_ => !Importing, FileImporterService.Import);
			DeleteBClick = new RelayCommand(_ => !Importing && FileViewModels.Count != 0, FileImporterService.DeleteBClick);
			SaveBClick = new RelayCommand(_ => !Importing && FileViewModels.Count != 0, FileImporterService.SaveBClick);

			FileViewModels.CollectionChanged += (_, _) =>
			{
				DeleteBClick.OnCanExecuteChanged();
				SaveBClick.OnCanExecuteChanged();
			};
		}

		public RelayCommand Import { get; }
		public RelayCommand DeleteBClick { get; }
		public RelayCommand SaveBClick { get; }
		public ObservableCollection<FileViewModel> FileViewModels { get; } = new();
		private bool _importing;

		public bool Importing
		{
			get => _importing;
			set
			{
				if (Equals(_importing, value)) return;
				_importing = value;
				Import.OnCanExecuteChanged();
				DeleteBClick.OnCanExecuteChanged();
				SaveBClick.OnCanExecuteChanged();
			}
		}


	}
}
