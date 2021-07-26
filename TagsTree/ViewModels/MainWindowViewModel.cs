using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TagsTree.Annotations;
using TagsTree.Models;
using TagsTree.Services;


namespace TagsTree.ViewModels
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public readonly Func<bool> CheckConfig = MainWindowService.CheckConfig;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen = MainWindowService.SuggestionChosen;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged = MainWindowService.TextChanged;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted = MainWindowService.QuerySubmitted;
		private ObservableCollection<FileModel> _fileModels = new();

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
		public ObservableCollection<FileModel> FileModels
		{
			get => _fileModels;
			set
			{
				if (Equals(_fileModels, value)) return;
				_fileModels = value;
				OnPropertyChanged(nameof(FileModels));
			}
		}

	}
}
