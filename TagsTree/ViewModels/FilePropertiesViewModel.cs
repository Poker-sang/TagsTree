using JetBrains.Annotations;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using TagsTree.Commands;
using TagsTree.Services;

namespace TagsTree.ViewModels
{
	public class FilePropertiesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void Load(FileViewModel file)
		{
			FileViewModel = new FileViewModel(file);
			Exists = File.Exists(file.FullName);
			if(Exists)
				Icon = App.CIconOfPath.IconOfPathLarge(file.FullName, true, true);
			OnPropertyChanged(nameof(FileViewModel));
			OnPropertyChanged(nameof(Icon));
		}

		public FilePropertiesViewModel()
		{
			OpenBClick = new RelayCommand(_ => _exists, FilePropertiesService.OpenBClick);
			RenameBClick = new RelayCommand(_ => _exists, FilePropertiesService.RenameBClick);
			MoveBClick = new RelayCommand(_ => _exists, FilePropertiesService.MoveBClick);
			DeleteBClick = new RelayCommand(_ => _exists, FilePropertiesService.DeleteBClick);
		}

		public RelayCommand OpenBClick { get; }
		public RelayCommand OpenExplorerBClick { get; } = new(_ => true, FilePropertiesService.OpenExplorerBClick);
		public RelayCommand EditTagsBClick { get; } = new(_ => true, FilePropertiesService.EditTagsBClick);
		public RelayCommand RemoveBClick { get; } = new(_ => true, FilePropertiesService.RemoveBClick);
		public RelayCommand RenameBClick { get; }
		public RelayCommand MoveBClick { get; }
		public RelayCommand DeleteBClick { get; }

		public FileViewModel FileViewModel { get; private set; }

		private bool _exists;

		public ImageSource? Icon { get; private set; }

		public bool Exists
		{
			get => _exists;
			set
			{
				if (Equals(_exists, value)) return;
				_exists = value;
				OnPropertyChanged(nameof(Exists));
				OpenExplorerBClick.OnCanExecuteChanged();
				RenameBClick.OnCanExecuteChanged();
				MoveBClick.OnCanExecuteChanged();
				DeleteBClick.OnCanExecuteChanged();
			}
		}
	}
}
