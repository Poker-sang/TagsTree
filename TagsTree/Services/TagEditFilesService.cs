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

		public static void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => Win.TbPath.Path = App.TagMethods.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? Win.TbPath.Path;

		public static void ResultChanged(TagSearchBox sender, ResultChangedEventArgs e)
		{
			var temp = e.NewResult.ToObservableCollection();
			foreach (var fileViewModel in temp)
				fileViewModel.Selected = fileViewModel.HasTag(Win.TbPath.Path.GetTagModel()!);
			((TagEditFilesViewModel)Win.DataContext).FileViewModels = temp;
		}

		public static void Selected(object sender, SelectionChangedEventArgs e)
		{
			if ((FileViewModel)((DataGrid)sender).SelectedItem is null) return;
			((FileViewModel)((DataGrid)sender).SelectedItem).Selected = ((FileViewModel)((DataGrid)sender).SelectedItem).Selected switch
			{
				true => false,
				false => true,
				null => false
			};
			((DataGrid)sender).SelectedIndex = -1;
		}

		#endregion

		#region 命令

		private static bool _mode;

		public static void ConfirmBClick(object? parameter)
		{
			if (!_mode)
			{
				if (Win.TbPath.Path.GetTagModel() is not { } pathTagModel)
				{
					App.MessageBoxX.Error("「标签路径」不存在！");
					return;
				}
				if (pathTagModel.XmlElement == App.XdpRoot)
				{
					App.MessageBoxX.Error("「标签路径」不能为空！");
					return;
				}
				Win.BConfirmClick();
				Vm.FileViewModels = App.Relations.GetFileModels().Select(fileModel => new FileViewModel(fileModel, pathTagModel)).ToObservableCollection();
				Win.TbPath.IsEnabled = false;
				_mode = true;
			}
			else
			{
				if (Win.TbPath.Path.GetTagModel() is not { } pathTagModel)
				{
					App.MessageBoxX.Error("「标签路径」不存在！"); //理论上不会到达此代码
					return;
				}
				foreach (var fileViewModel in Vm.FileViewModels)
					switch (fileViewModel.Selected)
					{
						case true:
						case null:
							switch (fileViewModel.HasTag(pathTagModel))
							{
								case true: //如果原本是true或null，则不改变
								case null: break;
								case false: //如果原本是false，则加上本标签
									App.Relations[fileViewModel, pathTagModel.Name] = true;
									fileViewModel.TagsUpdated();
									break;
							}
							break;
						case false:
							switch (fileViewModel.HasTag(pathTagModel))
							{
								case true:  //如果原本是true，则删除本标签
									App.Relations[fileViewModel, pathTagModel.Name] = false;
									fileViewModel.TagsUpdated();
									break;
								case null: //如果原本是null，则删除fileViewModel拥有的相应子标签
									App.Relations[fileViewModel, fileViewModel.GetRelativeTag(pathTagModel)!.Name] = false;
									fileViewModel.TagsUpdated();
									break;
								case false: break;//如果原本是false，则不改变
							}
							break;
					}
				App.SaveRelations();
				App.MessageBoxX.Information("已保存更改");
			}
		}

		#endregion
	}
}