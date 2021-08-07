using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TagsTree.Delegates;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using TagsTree.Views;
using TagsTree.Views.Controls;

namespace TagsTree.Services
{
	public static class TagEditFilesService
	{
		private static TagEditFilesViewModel Vm;
		private static TagEditFiles Win;

		public static void Load(TagEditFiles window)
		{
			Win = window;
			Vm = (TagEditFilesViewModel)window.DataContext;
			_mode = false;
		}

		#region 事件处理

		public static void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => Win.TbPath.AutoSuggestBox.Text = App.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? Win.TbPath.AutoSuggestBox.Text;

		public static void ResultChanged(TagSearchBox sender, ResultChangedEventArgs e) => ((TagEditFilesViewModel)Win.DataContext).FileViewModels = e.NewResult.ToObservableCollection();

		public static void Selected(object sender, SelectionChangedEventArgs e)
		{
			if((FileViewModel)((DataGrid)sender).SelectedItem is null) return;
			((FileViewModel)((DataGrid)sender).SelectedItem).Selected = !((FileViewModel)((DataGrid)sender).SelectedItem).Selected;
			((DataGrid)sender).SelectedIndex = -1;
		}

		#endregion

		#region 命令

		private static bool _mode;

		public static void ConfirmBClick(object? parameter)
		{
			if (!_mode)
			{
				if (App.IsTagNotExists(Win.TbPath.AutoSuggestBox.Text))
					return;
				Win.BConfirmClick();
				Vm.FileViewModels = App.Relations.GetFileModels(new List<string>()).Select(fileModel => new FileViewModel(fileModel, Win.TbPath.AutoSuggestBox.Text)).ToObservableCollection();
				_mode = true;
			}
			else
			{
				foreach (var fileViewModel in Vm.FileViewModels) 
					App.Relations[fileViewModel, Win.TbPath.AutoSuggestBox.Text.Split('\\', StringSplitOptions.RemoveEmptyEntries).Last()] = fileViewModel.Selected;
				App.SaveRelations();
				App.MessageBoxX.Information("已保存更改");
			}
		}

		#endregion
	}
}