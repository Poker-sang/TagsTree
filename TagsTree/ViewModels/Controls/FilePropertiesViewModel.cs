using JetBrains.Annotations;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using TagsTree.Commands;
using TagsTree.Services.Controls;

namespace TagsTree.ViewModels.Controls
{
	public class FilePropertiesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void Load(FileViewModel file)
		{
			FileViewModel = file;
			OnPropertyChanged(nameof(FileViewModel));
			OpenExplorerBClick.OnCanExecuteChanged();
			RenameBClick.OnCanExecuteChanged();
			MoveBClick.OnCanExecuteChanged();
			DeleteBClick.OnCanExecuteChanged();
		}

		public FilePropertiesViewModel()
		{
			OpenBClick = new RelayCommand(_ => FileViewModel.Exists, FilePropertiesService.OpenBClick);
			RenameBClick = new RelayCommand(_ => FileViewModel.Exists, FilePropertiesService.RenameBClick);
			MoveBClick = new RelayCommand(_ => FileViewModel.Exists, FilePropertiesService.MoveBClick);
			DeleteBClick = new RelayCommand(_ => FileViewModel.Exists, FilePropertiesService.DeleteBClick);
		}

		public RelayCommand OpenBClick { get; }
		public RelayCommand OpenExplorerBClick { get; } = new(_ => true, FilePropertiesService.OpenExplorerBClick);
		public RelayCommand EditTagsBClick { get; } = new(_ => true, FilePropertiesService.EditTagsBClick);
		public RelayCommand RemoveBClick { get; } = new(_ => true, FilePropertiesService.RemoveBClick);
		public RelayCommand RenameBClick { get; }
		public RelayCommand MoveBClick { get; }
		public RelayCommand DeleteBClick { get; }

		public FileViewModel FileViewModel { get; private set; }
	}
}
