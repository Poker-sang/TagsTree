using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
		public static void ResultChanged(TagSuggestBox sender, ResultChangedEventArgs e) => ((TagAddFilesViewModel)Win.DataContext).FileViewModels = e.NewResult.ToObservableCollection();
		
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