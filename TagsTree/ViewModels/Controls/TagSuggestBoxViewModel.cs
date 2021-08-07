using JetBrains.Annotations;
using ModernWpf;
using ModernWpf.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TagsTree.Services.Controls;

namespace TagsTree.ViewModels.Controls
{
	public class TagSuggestBoxViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen => TagSuggestBoxService.SuggestionChosen;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged => TagSuggestBoxService.TextChanged;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted => TagSuggestBoxService.QuerySubmitted;

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
