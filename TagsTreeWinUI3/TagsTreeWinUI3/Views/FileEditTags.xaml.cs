using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Views
{
	/// <summary>
	/// FileEditTags.xaml 的交互逻辑
	/// </summary>
	public partial class FileEditTags : Window
	{
		public FileEditTags(FileViewModel fileViewModel)
		{
			InitializeComponent();
			fileViewModel.VirtualTagsInitialize();
			_vm = new FileEditTagsViewModel(fileViewModel);
		}

		private readonly FileEditTagsViewModel _vm;

		private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = ((TagModel?)e.InvokedItem)?.FullName ?? TbPath.Path;

		private async void AddBClick(object parameter, RoutedEventArgs routedEventArgs)
		{
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageDialogX.Information(true, "「标签路径」不存在！");
				return;
			}
			if (_vm.FileViewModel.VirtualTags.GetTagModels().Contains(pathTagModel))
			{
				MessageDialogX.Information(true, "已拥有该标签");
				return;
			}
			if (_vm.FileViewModel.GetRelativeVirtualTag(pathTagModel) is { } relativeTag)
			{
				MessageDialogX.Information(true, $"已拥有下级标签「{relativeTag}」");
				return;
			}
			foreach (var tagModel in _vm.FileViewModel.VirtualTags.GetTagModels())
				if (tagModel.HasChildTag(pathTagModel))
				{
					if (await MessageDialogX.Warning($"将会覆盖上级标签「{tagModel.Name}」，是否继续？"))
						_vm.FileViewModel.VirtualTags = $" {_vm.FileViewModel.VirtualTags} ".Replace($" {tagModel.Name} ", " ").Trim();
					else return;
				}
			_vm.FileViewModel.VirtualTags += (_vm.FileViewModel.VirtualTags is "" ? "" : " ") + pathTagModel.Name;
			BSave.IsEnabled = true;
		}

		private void DeleteBClick(object parameter, RoutedEventArgs e)
		{
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageDialogX.Information(true, "「标签路径」不存在！");
				return;
			}
			_vm.FileViewModel.VirtualTags = $" {_vm.FileViewModel.VirtualTags} ".Replace($" {pathTagModel.Name} ", " ").Trim();
			BSave.IsEnabled = true;
		}

		private void SaveBClick(object parameter, RoutedEventArgs e)
		{
			for (var index = 1; index < App.Relations.Columns.Count; index++)
			{
				var column = App.Relations.Columns[index];
				App.Relations.RowAt(_vm.FileViewModel.GetFileModel)[column] = $" {_vm.FileViewModel.VirtualTags} ".Contains($" {App.Tags.TagsDictionary[Convert.ToInt32(column.ColumnName)].Name} ");
			}
			_vm.FileViewModel.TagsUpdated();
			App.SaveRelations();
			MessageDialogX.Information(false, "已保存更改");
			BSave.IsEnabled = false;
		}

	}
}
