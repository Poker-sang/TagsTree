using JetBrains.Annotations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TagsTree.Commands;
using TagsTree.Delegates;
using TagsTree.Services;
using TagsTree.Views.Controls;


namespace TagsTree.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public RelayCommand OpenCmClick { get; } = new(_ => true, MainService.OpenCmClick);
		public RelayCommand OpenExplorerCmClick { get; } = new(_ => true, MainService.OpenExplorerCmClick);
		public RelayCommand RemoveFileCmClick { get; } = new(_ => true, MainService.RemoveCmClick);
		public RelayCommand PropertiesCmClick { get; } = new(_ => true, MainService.PropertiesCmClick);

		public static Func<bool> CheckConfig => MainService.CheckConfig;
		public static MouseButtonEventHandler MainMouseLeftButtonDown => MainService.MainMouseLeftButtonDown;
		public static MouseButtonEventHandler DgItemMouseDoubleClick => MainService.DgItemMouseDoubleClick;
		public static ResultChangedEventHandler ResultChanged => MainService.ResultChanged;
		public static FileRemovedEventHandler FileRemoved => MainService.FileRemoved;

		private ObservableCollection<FileViewModel> _fileViewModels;
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
		public void CollectionChanged() => OnPropertyChanged(nameof(FileViewModels));
	}
}
