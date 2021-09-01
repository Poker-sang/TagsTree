using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TagsTreeWinUI3.ViewModels
{
	public class FileEditTagsViewModel : ObservableObject
	{
		public void Load(FileViewModel fileViewModel)
		{
			_fileViewModel = fileViewModel;
			_fileViewModel.VirtualTagsInitialize();
		}

		public ObservableCollection<TagViewModel> TagsSource { get; set; } = App.Tags.TagsTree.SubTags;

		private FileViewModel _fileViewModel = null!;

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
	}
}
