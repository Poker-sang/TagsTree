using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModernWpf.Controls;
using TagsTree.Models;
using TagsTree.ViewModels;
using TagsTree.Views;

namespace TagsTree.Services
{
	public static class MainWindowService
	{
		private static readonly MainWindowViewModel Vm = new();
		private static MainWindow Win;

		public static MainWindowViewModel Load(MainWindow window)
		{
			Win = window;
			return Vm;
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
			Vm.Search = Regex.Replace(Vm.Search, @"[\\\/\:\*\?\""\<\>\|]+", "");
			Vm.Search = Regex.Replace(Vm.Search, @"  +", " ").TrimStart();
			sender.ItemsSource = App.TagSuggest(sender.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
		}
		public static void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{

		}
    }
}
