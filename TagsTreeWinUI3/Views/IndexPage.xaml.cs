using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Views
{
	public partial class IndexPage : Page
	{
		public IndexPage()
		{
			_vm = new IndexViewModel();
			_current = this;
			InitializeComponent();
			Storyboard0.Begin();
			//_ = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => Keyboard.Focus(TbSearch)));
		}

		private readonly IndexViewModel _vm;

		private bool _isSearched;
		private void Search(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			if (_isSearched) return;
			_isSearched = true;
			Storyboard1.Begin();
		}
		private void Storyboard1_OnCompleted(object sender, object e)
		{
			Grid.SetRow(TbSearch, 1);
			TbSearch.VerticalAlignment = VerticalAlignment.Top;
			Storyboard2.Begin();
		}

		private void Storyboard2_OnCompleted(object sender, object e)
		{
			TbFuzzySearch.IsHitTestVisible = true;
			DgResult.IsHitTestVisible = true;
		}


		#region 事件处理

		private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.ResultCallBack = newResult.Select(fileModel => new FileViewModel(fileModel)).ToObservableCollection();

		private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => sender.Text = Regex.Replace(sender.Text, $@"[{FileX.GetInvalidNameChars} ]+", "");

		private void QuerySubmitted(AutoSuggestBox autoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs e) => _vm.FileViewModels = autoSuggestBox.Text is "" ? _vm.ResultCallBack : RelationsDataTable.FuzzySearchName(autoSuggestBox.Text, _vm.ResultCallBack);

		private void OpenCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).Tag).Open();
		private void OpenExplorerCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).Tag).OpenDirectory();
		private async void RemoveCmClick(object sender, RoutedEventArgs e)
		{
			if (!await MessageDialogX.Warning("是否从软件移除该文件？")) return;
			((FileViewModel)((FrameworkElement)sender).Tag).RemoveAndSave();
			_ = _vm.FileViewModels.Remove((FileViewModel)((FrameworkElement)sender).Tag);
		}
		private void PropertiesCmClick(object sender, RoutedEventArgs e) => PropertiesCmDoubleClick(((MenuFlyoutItem)sender).Tag, e);

		private void PropertiesCmDoubleClick(object sender, RoutedEventArgs e) => App.RootFrame.Navigate(typeof(FilePropertiesPage), (FileViewModel)((FrameworkElement)sender).Tag);

		#endregion

		#region 操作

		private static IndexPage _current = null!;
		public static void FileRemoved(FileViewModel removedItem) => _ = _current._vm.FileViewModels.Remove(removedItem);

		#endregion
	}
}