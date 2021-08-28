using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TagsTreeWinUI3.Commands;
using TagsTreeWinUI3.Views;

namespace TagsTreeWinUI3.ViewModels
{
	public sealed class FileImporterViewModel : ObservableObject
	{
		public FileImporterViewModel(FileImporterPage page)
		{
			Import = new RelayCommand(_ => !Importing, page.Import);
			DeleteBClick = new RelayCommand(_ => !Importing && FileViewModels.Count != 0, page.DeleteBClick);
			SaveBClick = new RelayCommand(_ => !Importing && FileViewModels.Count != 0, page.SaveBClick);

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
