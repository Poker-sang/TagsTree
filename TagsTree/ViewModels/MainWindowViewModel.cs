using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ModernWpf.Controls;
using ModernWpf;
using TagsTree.Annotations;
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
