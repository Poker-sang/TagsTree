using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Delegates;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;
using Windows.Foundation;

namespace TagsTreeWinUI3.Views.Controls
{
	/// <summary>
	/// TagSearchBox.xaml 的交互逻辑
	/// </summary>
	public partial class TagSearchBox : UserControl
	{

		public TagSearchBox() => InitializeComponent();

		public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> BeforeQuerySubmitted
		{
			add
			{
				AutoSuggestBox.QuerySubmitted -= QuerySubmitted;
				AutoSuggestBox.QuerySubmitted += value;
				AutoSuggestBox.QuerySubmitted += QuerySubmitted;
			}
			remove
			{
				AutoSuggestBox.QuerySubmitted -= QuerySubmitted;
				AutoSuggestBox.QuerySubmitted -= value;
				AutoSuggestBox.QuerySubmitted += QuerySubmitted;
			}
		}

		public event ResultChangedEventHandler ResultChanged = null!;

		private void SuggestionChosen(AutoSuggestBox autoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs e)
		{
			var index = autoSuggestBox.Text.LastIndexOf(' ') + 1;
			if (index == 0)
				autoSuggestBox.Text = e.SelectedItem.ToString();
			else autoSuggestBox.Text = autoSuggestBox.Text[..index] + e.SelectedItem;
		}
		private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			if (sender.Text is "")
			{
				ResultChanged.Invoke(App.IdFile.Values.Select(fileModel => new FileViewModel(fileModel)));
				return;
			}
			sender.Text = Regex.Replace(sender.Text, $@"[{FileX.GetInvalidPathChars}]+", "");
			sender.Text = Regex.Replace(sender.Text, @"  +", " ").TrimStart();
			sender.ItemsSource = sender.Text.TagSuggest(' ');
		}
		private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => ResultChanged.Invoke(App.Relations.GetFileModels(sender.Text.GetTagsFiles().ToList()));
	}
}
