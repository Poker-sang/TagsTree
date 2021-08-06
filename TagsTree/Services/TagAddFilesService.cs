using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TagsTree.ViewModels;
using TagsTree.Views;

namespace TagsTree.Services
{
	public static class TagAddFilesService
	{
		private static TagAddFilesViewModel Vm;
		private static TagAddFiles Win;
		public static void Load(TagAddFiles window)
		{
			Win = window;
			Vm = (TagAddFilesViewModel)window.DataContext;
			_mode = false;
		}

		#region 事件处理

		public static void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			Vm.SelectedTag = App.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? Vm.SelectedTag;
			Vm.CanConfirm = true;
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
			Vm.FileViewModels.Clear();
			var tags = sender.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var validTags = new Dictionary<string, bool>();
			foreach (var tag in tags)
				if (!validTags.ContainsKey(tag))
					validTags[tag] = true;
			foreach (var fileModel in App.Relations.GetFileModels(validTags.Keys.ToList()))
				Vm.FileViewModels.Add(new FileViewModel(fileModel));
		}

		#endregion

		#region 命令

		private static bool _mode;
		public static void ConfirmBClick(object? parameter)
		{
			if (!_mode)
			{
				Win.BConfirmClick();
				foreach (var fileModel in App.IdFile.Values.ToList())
					Vm.FileViewModels.Add(new FileViewModel(fileModel));
				_mode = true;
			}
			else
			{
				foreach (var fileViewModel in Vm.FileViewModels)
					App.Relations[fileViewModel, Vm.SelectedTag.Split('\\', StringSplitOptions.RemoveEmptyEntries).Last()] = fileViewModel.Selected;
				App.SaveRelations();
				App.MessageBoxX.Information("已保存更改");
			};
		}

		#endregion

		#region 操作

		public static void Selected(object fileViewModel) => ((FileViewModel)fileViewModel).Selected = !((FileViewModel)fileViewModel).Selected;

		#endregion
	}
}