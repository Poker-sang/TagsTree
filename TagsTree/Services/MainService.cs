using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TagsTree.ViewModels;
using TagsTree.Views;
using static TagsTree.Properties.Settings;

namespace TagsTree.Services
{
	public static class MainService
	{
		private static MainViewModel Vm;
		public static Main Win;

		public static void Load(Main window)
		{
			Win = window;
			Vm = (MainViewModel)window.DataContext;
		}

		public static void LoadFileProperties() => _fileProperties = Win.FileProperties;

		private static FileProperties _fileProperties;
		private static bool _isShowed;

		#region 事件处理

		public static void MainMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!_isShowed || e.GetPosition((Main)sender).X is >= 215 and <= 1065) return;
			_fileProperties.Hide();
			_isShowed = false;
		}

		public static void DgItemMouseDoubleClick(object sender, MouseButtonEventArgs e) => PropertiesCmClick(sender);

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
			Vm.FileModels.Clear();
			var tags = sender.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var validTags = new Dictionary<string, bool>();
			foreach (var tag in tags)
				if (!validTags.ContainsKey(tag))
					validTags[tag] = true;
			foreach (var fileModel in App.Relations.GetFileModels(validTags.Keys.ToList()))
				Vm.FileModels.Add(new FileViewModel(fileModel));
		}

		#endregion

		#region 命令

		public static void OpenCmClick(object? parameter) => App.FileX.Open(((FileViewModel)((DataGridRow)parameter!).DataContext).FullName);
		public static void OpenExplorerCmClick(object? parameter) => App.FileX.Open(((FileViewModel)((DataGridRow)parameter!).DataContext).Path);
		public static void RemoveCmClick(object? parameter)
		{
			if (!App.MessageBoxX.Warning("是否从软件移除该文件？")) return;
			var value = (FileViewModel)((DataGridRow)parameter!).DataContext;
			if (App.IdFile.Contains(value))
			{
				_ = App.IdFile.Remove(value);
				App.Relations.Rows.Remove(App.Relations.RowAt(value));
				App.Relations.RefreshRowsDict();
				App.SaveFiles();
			}

			_ = Vm.FileModels.Remove((FileViewModel)((DataGridRow)parameter).DataContext);
		}
		public static async void PropertiesCmClick(object? parameter)
		{
			((FilePropertiesViewModel)Win.FileProperties.Grid.DataContext).Load((FileViewModel)((DataGridRow)parameter!).DataContext);
			_ = _fileProperties.ShowAsync(ContentDialogPlacement.Popup);
			await Task.Delay(100);
			_isShowed = true;
		}

		#endregion

		#region 操作

		public static bool CheckConfig()
		{
			while (true)
			{
				if (Default.IsSet) //如果之前有储存过用户配置，则判断是否符合
					switch (App.LoadConfig())
					{
						case null: return false;
						case true: return true;
					}
				else if (new NewConfig().ShowDialog() == false)
					return false;
			}
		}

		#endregion
	}
}
