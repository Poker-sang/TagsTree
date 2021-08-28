using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TagsTreeWinUI3.Models;

namespace TagsTreeWinUI3.ViewModels
{
	public class FileEditTagsViewModel : ObservableObject
	{
		public FileEditTagsViewModel(FileViewModel fileViewModel)
		{
			_fileViewModel = fileViewModel;
		}

		public ObservableCollection<TagModel> TagsSource { get; set; } = App.Tags.TagsTree.SubTags;

		private FileViewModel _fileViewModel;

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
