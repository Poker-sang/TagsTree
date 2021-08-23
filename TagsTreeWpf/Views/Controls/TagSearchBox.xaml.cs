using ModernWpf;
using ModernWpf.Controls;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TagsTreeWpf.Delegates;
using TagsTreeWpf.Services;
using TagsTreeWpf.Services.ExtensionMethods;
using TagsTreeWpf.ViewModels;

namespace TagsTreeWpf.Views.Controls
{
	/// <summary>
	/// TagSearchBox.xaml 的交互逻辑
	/// </summary>
	public partial class TagSearchBox : UserControl
	{

		public TagSearchBox() => InitializeComponent();

		public TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> BeforeQuerySubmitted
		{
			set
			{
				AutoSuggestBox.QuerySubmitted -= QuerySubmitted;
				AutoSuggestBox.QuerySubmitted += value;
				AutoSuggestBox.QuerySubmitted += QuerySubmitted;
			}
		}

		public event ResultChangedEventHandler ResultChanged;

		private void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
		{
			var index = sender.Text.LastIndexOf(' ') + 1;
			if (index == 0)
				sender.Text = e.SelectedItem.ToString();
			else sender.Text = sender.Text[..index] + e.SelectedItem;
		}
		private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			if (sender.Text is "")
			{
				ResultChanged.Invoke(this, new ResultChangedEventArgs(App.IdFile.Values.Select(fileModel => new FileViewModel(fileModel))));
				return;
			}
			sender.Text = Regex.Replace(sender.Text, $@"[{FileX.GetInvalidPathChars}]+", "");
			sender.Text = Regex.Replace(sender.Text, @"  +", " ").TrimStart();
			sender.ItemsSource = TagMethods.TagSuggest(sender.Text, ' ');
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}
		private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => ResultChanged.Invoke(this, new ResultChangedEventArgs(App.Relations.GetFileModels(sender.Text.GetTagsFiles().ToList())));
	}
}
