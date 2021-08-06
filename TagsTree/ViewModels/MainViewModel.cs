using JetBrains.Annotations;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TagsTree.Commands;
using TagsTree.Services;


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
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen => MainService.SuggestionChosen;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged => MainService.TextChanged;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted => MainService.QuerySubmitted;

		public ObservableCollection<FileViewModel> FileModels { get; } = new();
		private string _search = "";

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
