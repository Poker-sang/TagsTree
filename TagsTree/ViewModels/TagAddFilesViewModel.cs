using JetBrains.Annotations;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using TagsTree.Commands;
using TagsTree.Services;

namespace TagsTree.ViewModels
{
	public class TagAddFilesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public TagAddFilesViewModel()
		{
			App.XdTagsReload();
			Xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
			ConfirmBClick = new RelayCommand(_ => CanConfirm, TagAddFilesService.ConfirmBClick);
		}

		public static RoutedPropertyChangedEventHandler<object> TvSelectItemChanged => TagAddFilesService.TvSelectItemChanged;
		public static Action<object> Selected => TagAddFilesService.Selected;
		public RelayCommand ConfirmBClick { get; }
		private bool _canConfirm;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen => TagAddFilesService.SuggestionChosen;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged => TagAddFilesService.TextChanged;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted => TagAddFilesService.QuerySubmitted;

		private string _selectedTag = "";
		private string _search = "";
		public ObservableCollection<FileViewModel> FileViewModels { get; } = new();
		public XmlDataProvider Xdp { get; }


		public bool CanConfirm
		{
			get => _canConfirm;
			set
			{
				if (Equals(_canConfirm, value)) return;
				_canConfirm = value;
				ConfirmBClick.OnCanExecuteChanged();
			}
		}
		public string SelectedTag
		{
			get => _selectedTag;
			set
			{
				if (Equals(_selectedTag, value)) return;
				_selectedTag = value;
				OnPropertyChanged(nameof(SelectedTag));
			}
		}
		public string Search
		{
			get => _search;
			set
			{
				if (Equals(_search, value)) return;
				_search = value;
				OnPropertyChanged(nameof(Search));
			}
		}
	}
}