using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

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
			App.XdTagsReload();
			Xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
		}

		public XmlDataProvider Xdp { get; }

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
