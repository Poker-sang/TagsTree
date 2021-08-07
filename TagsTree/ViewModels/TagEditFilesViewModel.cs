using JetBrains.Annotations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TagsTree.Commands;
using TagsTree.Delegates;
using TagsTree.Services;

namespace TagsTree.ViewModels
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

		public static RoutedPropertyChangedEventHandler<object> TvSelectItemChanged => TagEditFilesService.TvSelectItemChanged;
		public static SelectionChangedEventHandler Selected => TagEditFilesService.Selected;
		public static ResultChangedEventHandler ResultChanged => TagEditFilesService.ResultChanged;

		public RelayCommand ConfirmBClick { get; } = new(_ => true, TagEditFilesService.ConfirmBClick);
		public XmlDataProvider Xdp { get; }
		
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
	}
}