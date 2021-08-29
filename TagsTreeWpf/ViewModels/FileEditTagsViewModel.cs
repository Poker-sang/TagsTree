using JetBrains.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TagsTreeWpf.Models;

namespace TagsTreeWpf.ViewModels
{
	public class FileEditTagsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
