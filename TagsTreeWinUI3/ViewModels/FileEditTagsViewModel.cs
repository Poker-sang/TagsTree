using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.ViewModels
{
	public class FileEditTagsViewModel : ObservableObject
	{
		public void Load(FileViewModel fileViewModel)
		{
			_fileViewModel = fileViewModel;
			VirtualTags = App.Relations.GetTags(fileViewModel).ToObservableCollection();
		}

		public static ObservableCollection<TagViewModel> TagsSource => App.Tags.TagsTree.SubTags;


		private FileViewModel _fileViewModel = null!;
		private ObservableCollection<TagViewModel> _virtualTags = null!;

		public FileViewModel FileViewModel
		{
			get => _fileViewModel;
			set
			{
				if (Equals(_fileViewModel, value)) return;
				_fileViewModel = value;
				OnPropertyChanged(nameof(FileViewModel));
			}
		}
		public ObservableCollection<TagViewModel> VirtualTags
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
