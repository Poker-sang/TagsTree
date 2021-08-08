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
using TagsTree.ViewModels.Controls;
using TagsTree.Views.Controls;

namespace TagsTree.Services.Controls
{
	public static class TagSearchBoxService
	{
		private static TagSearchBoxViewModel Vm;
		private static TagSearchBox TagSearchBox;
		public static void Load(TagSearchBox tagSuggestBox)
		{
			Vm = (TagSearchBoxViewModel)tagSuggestBox.AutoSuggestBox.DataContext;
			TagSearchBox = tagSuggestBox;
		}
		public static void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
		{
			var index = sender.Text.LastIndexOf(' ') + 1;
			if (index == 0)
				sender.Text = e.SelectedItem.ToString();
			else sender.Text = sender.Text[..index] + e.SelectedItem;
		}
		public static void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			Vm.Search = Regex.Replace(Vm.Search, $@"[{App.FileX.GetInvalidPathChars}]+", "");
			Vm.Search = Regex.Replace(Vm.Search, @"  +", " ").TrimStart();
			sender.ItemsSource = App.TagSuggest(sender.Text, ' ');
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}
		public static void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			var tags = sender.Text.GetTagModels();
			var validTags = new Dictionary<TagModel, bool>();
			foreach (var tag in tags)
				if (!validTags.ContainsKey(tag))
					validTags[tag] = true;
			TagSearchBox.OnResultChanged(new ResultChangedEventArgs(App.Relations.GetFileModels(validTags.Keys.ToList()).Select(fileModel => new FileViewModel(fileModel))));
		}
	}
}
