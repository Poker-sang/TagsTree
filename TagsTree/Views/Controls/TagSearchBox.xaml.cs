using ModernWpf;
using ModernWpf.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TagsTree.Delegates;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views.Controls
{
	/// <summary>
	/// TagSearchBox.xaml 的交互逻辑
	/// </summary>
	public partial class TagSearchBox : UserControl
	{

		public TagSearchBox()
		{
			InitializeComponent();
		}

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
			AutoSuggestBox.Text = Regex.Replace(AutoSuggestBox.Text, $@"[{App.FileX.GetInvalidPathChars}]+", "");
			AutoSuggestBox.Text = Regex.Replace(AutoSuggestBox.Text, @"  +", " ").TrimStart();
			sender.ItemsSource = App.TagMethods.TagSuggest(sender.Text, ' ');
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}
		private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			var tags = sender.Text.GetTagModels();
			var validTags = new Dictionary<TagModel, bool>();
			foreach (var tag in tags)
				if (!validTags.ContainsKey(tag))
					validTags[tag] = true;
			ResultChanged.Invoke(this, new ResultChangedEventArgs(App.Relations.GetFileModels(validTags.Keys.ToList()).Select(fileModel => new FileViewModel(fileModel))));
		}
	}
}
