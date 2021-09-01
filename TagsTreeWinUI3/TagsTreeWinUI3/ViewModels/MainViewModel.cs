using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using TagsTreeWinUI3.Services.ExtensionMethods;


namespace TagsTreeWinUI3.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		private ObservableCollection<FileViewModel> _resultCallBack = Enumerable.Empty<FileViewModel>().ToObservableCollection();
		private ObservableCollection<FileViewModel> _fileViewModels = Enumerable.Empty<FileViewModel>().ToObservableCollection();
		public ObservableCollection<FileViewModel> ResultCallBack
		{
			get => _resultCallBack;
			set
			{
				if (Equals(_resultCallBack, value)) return;
				_resultCallBack = value;
				FileViewModels = value;
			}
		}

		public ObservableCollection<FileViewModel> FileViewModels
		{
			get => _fileViewModels;
			set
			{
				if (Equals(_fileViewModels, value)) return;
				_fileViewModels = value;
				OnPropertyChanged(nameof(FileViewModels));
			}
		}
	}
}
