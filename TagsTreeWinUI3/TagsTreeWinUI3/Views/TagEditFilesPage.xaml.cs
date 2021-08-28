using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using TagsTreeWinUI3.Delegates;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;
using TagsTreeWinUI3.Views.Controls;

namespace TagsTreeWinUI3.Views
{
	/// <summary>
	/// TagEditFilesPage.xaml 的交互逻辑
	/// </summary>
	public partial class TagEditFilesPage : Page
	{
		public TagEditFilesPage()
		{
			_vm = new TagEditFilesViewModel();
			InitializeComponent();
		}

		private readonly TagEditFilesViewModel _vm;

		private void BConfirmClick()
		{
			//Tags.BeginAnimation(OpacityProperty, new DoubleAnimation
			//{
			//	From = 1,
			//	To = 0,
			//	Duration = TimeSpan.FromMilliseconds(500)
			//});
			//await Task.Delay(500);
			//MessageDialogX.Children.Remove(Tags);
			//BConfirm.Content = "保存";
			//TbInput.BeginAnimation(OpacityProperty, new DoubleAnimation
			//{
			//	From = 0,
			//	To = 1,
			//	Duration = TimeSpan.FromMilliseconds(500)
			//});
			//DgResult.BeginAnimation(OpacityProperty, new DoubleAnimation
			//{
			//	From = 0,
			//	To = 1,
			//	Duration = TimeSpan.FromMilliseconds(500)
			//});
			//TbInput.IsHitTestVisible = true;
			//DgResult.IsHitTestVisible = true;
		}

		#region 事件处理

		//private void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => TbPath.Path = TbPath.Path.TvSelectedItemChanged((TagModel?)e.NewValue);

		private void ResultChanged(TagSearchBox sender, ResultChangedEventArgs e) => _vm.FileViewModels = e.NewResult.Select(fileModel => new FileViewModel(fileModel, TbPath.Path.GetTagModel()!)).ToObservableCollection();

		private void Selected(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			if ((FileViewModel)((DataGrid)sender).SelectedItem is null) return;
			((FileViewModel)((DataGrid)sender).SelectedItem).SelectedFlip();
			((DataGrid)sender).SelectedIndex = -1;
		}

		#endregion

		#region 命令

		private static bool _mode;

		private void ConfirmBClick(object parameter, RoutedEventArgs routedEventArgs)
		{
			if (!_mode)
			{
				if (TbPath.Path.GetTagModel() is not { } pathTagModel)
				{
					MessageDialogX.Information(true, "「标签路径」不存在！");
					return;
				}
				if (pathTagModel == App.Tags.TagsDictionaryRoot)
				{
					MessageDialogX.Information(true, "「标签路径」不能为空！");
					return;
				}
				BConfirmClick();
				_vm.FileViewModels = App.Relations.GetFileModels().Select(fileModel => new FileViewModel(fileModel, pathTagModel)).ToObservableCollection();
				TbPath.IsEnabled = false;
				_mode = true;
			}
			else
			{
				if (TbPath.Path.GetTagModel() is not { } pathTagModel)
				{
					MessageDialogX.Information(true, "「标签路径」不存在！"); //理论上不会到达此代码
					return;
				}
				foreach (var fileViewModel in _vm.FileViewModels)
					if (fileViewModel.Selected != fileViewModel.SelectedOriginal)
					{
						switch (fileViewModel.SelectedOriginal)
						{
							case true: App.Relations[fileViewModel, pathTagModel] = false; break;
							case false: App.Relations[fileViewModel, pathTagModel] = true; break;
							case null: //如果原本是null，则删除fileViewModel拥有的相应子标签
								foreach (var tag in fileViewModel.GetRelativeTags(pathTagModel))
									App.Relations[fileViewModel, tag] = false;
								break;
						}
						fileViewModel.TagsUpdated();
					}
				App.SaveRelations();
				MessageDialogX.Information(false, "已保存更改");
			}
		}

		#endregion
	}
}
