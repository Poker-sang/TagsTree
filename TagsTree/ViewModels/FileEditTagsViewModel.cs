using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using TagsTree.Commands;
using TagsTree.Services;

namespace TagsTree.ViewModels
{
	public class FileEditTagsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FileEditTagsViewModel()
		{
			App.XdTagsReload();
			Xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
			SaveBClick = new RelayCommand(_ => CanSave, FileEditTagsService.SaveBClick);
		}

		public RelayCommand AddBClick { get; } = new(_ => true, FileEditTagsService.AddBClick);
		public RelayCommand DeleteBClick { get; } = new(_ => true, FileEditTagsService.DeleteBClick);
		public RelayCommand SaveBClick { get; }

		public static RoutedPropertyChangedEventHandler<object> TvSelectItemChanged => FileEditTagsService.TvSelectItemChanged;
		public XmlDataProvider Xdp { get; }

		private bool _canSave;
		private FileViewModel _fileViewModel;

		public bool CanSave
		{
			get => _canSave;
			set
			{
				if (Equals(_canSave, value)) return;
				_canSave = value;
				SaveBClick.OnCanExecuteChanged();
			}
		}
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
