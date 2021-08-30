using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using TagsTreeWinUI3.Services.ExtensionMethods;


namespace TagsTreeWinUI3.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		public ObservableCollection<FileViewModel> FileViewModels { get; set; } = Enumerable.Empty<FileViewModel>().ToObservableCollection();

		private ObservableCollection<FileViewModel> _resultCallBack = Enumerable.Empty<FileViewModel>().ToObservableCollection();
		public ObservableCollection<FileViewModel> ResultCallBack
		{
			get => _resultCallBack;
			set
			{
				if (Equals(_resultCallBack, value)) return;
				_resultCallBack = value;
				FileViewModels = value;
				OnPropertyChanged(nameof(FileViewModels));
			}
		}
	}
}
