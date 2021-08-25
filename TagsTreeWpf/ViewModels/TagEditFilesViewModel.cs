using JetBrains.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using TagsTreeWpf.Services.ExtensionMethods;

namespace TagsTreeWpf.ViewModels
{
	public class TagEditFilesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public TagEditFilesViewModel()
		{
			App.XdTagsReload();
			Xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
		}

		public XmlDataProvider Xdp { get; }

		private ObservableCollection<FileViewModel> _fileViewModels = Enumerable.Empty<FileViewModel>().ToObservableCollection();

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