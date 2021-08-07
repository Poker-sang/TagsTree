using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TagsTree.Delegates;
using TagsTree.ViewModels;
using TagsTree.ViewModels.Controls;
using TagsTree.Views.Controls;

namespace TagsTree.Services.Controls
{
	public static class TagSuggestBoxService
	{
		private static TagSuggestBoxViewModel Vm;
		private static TagSuggestBox TagSuggestBox;
		public static void Load(TagSuggestBox tagSuggestBox)
		{
			Vm = (TagSuggestBoxViewModel)tagSuggestBox.AutoSuggestBox.DataContext;
			TagSuggestBox = tagSuggestBox;
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
			sender.ItemsSource = App.TagSuggest(sender.Text);
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}
		public static void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			var tags = sender.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var validTags = new Dictionary<string, bool>();
			foreach (var tag in tags)
				if (!validTags.ContainsKey(tag))
					validTags[tag] = true;
			TagSuggestBox.OnResultChanged(new ResultChangedEventArgs(App.Relations.GetFileModels(validTags.Keys.ToList()).Select(fileModel => new FileViewModel(fileModel))));
		}
	}
}
