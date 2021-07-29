using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using TagsTree.Models;
using TagsTree.ViewModels;
using TagsTree.Views;
using static TagsTree.Properties.Settings;

namespace TagsTree.Services
{
	public static class MainService
	{
		private static readonly MainViewModel Vm = new();
		private static Main Win;

		public static MainViewModel Load(Main window)
		{
			Win = window;
			return Vm;
		}


		#region 事件处理

		public static void DgItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			throw new NotImplementedException();
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
				if (App.TagPathComplete(tag) is not null && !validTags.ContainsKey(tag))
					validTags[tag] = true;
			foreach (var fileModel in App.Relations.GetFileModels(validTags.Keys.ToList()))
				Vm.FileModels.Add(fileModel);
		}

		#endregion

		#region 命令

		public static void OpenCmClick(object? parameter) => Open(((FileModel)((DataGridRow)parameter!).DataContext).FullName);
		public static void OpenExplorerCmClick(object? parameter) => Open(((FileModel)((DataGridRow)parameter!).DataContext).Path);

		public static void RemoveFileCmClick(object? parameter)
		{
			
			foreach (var (key, file) in App.IdToFile)
				if (file == (FileModel)((DataGridRow)parameter!).DataContext)
				{
					_ = App.IdToFile.Remove(key);
					App.Relations.Rows.Remove(App.Relations.RowAt(file));
					App.Relations.RefreshRowsDict();
					App.SaveFiles();
				}
			_ = Vm.FileModels.Remove((FileModel)((DataGridRow)parameter!).DataContext);
		}
		public static void PropertiesCmClick(object? parameter)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region 操作

		public static bool CheckConfig()
		{
			while (true)
			{
				if (Default.IsSet) //如果之前有储存过用户配置，则判断是否符合
					switch (App.LoadConfig(Default.ConfigPath))
					{
						case null: return false;
						case true: return true;
					}
				else if (new NewConfig().ShowDialog() == false)
					return false;
			}
		}

		private static void Open(string fileName)
		{

			try
			{
				var process = new Process { StartInfo = new ProcessStartInfo(fileName) };
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
			catch (System.ComponentModel.Win32Exception)
			{
				App.ErrorMessageBox("找不到文件夹，源文件可能已被更改");
			}
		}

		#endregion


	}
}
