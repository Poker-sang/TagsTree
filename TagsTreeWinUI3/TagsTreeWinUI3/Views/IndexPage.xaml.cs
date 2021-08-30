﻿using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagsTreeWinUI3.Delegates;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;
using TagsTreeWinUI3.Views.Controls;

namespace TagsTreeWinUI3.Views
{
	public partial class IndexPage : Page
	{
		public IndexPage()
		{
			_vm = new MainViewModel();
			InitializeComponent();

			//_ = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => Keyboard.Focus(TbSearch)));
		}

		private readonly MainViewModel _vm;

		private bool _isSearched;
		private void Search(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			if (_isSearched) return;
			_isSearched = true;
			Storyboard1.Begin();
		}
		private void Storyboard1_OnCompleted(object sender, object e) => Storyboard2.Begin();

		private void Storyboard2_OnCompleted(object sender, object e)
		{
			TbFuzzySearch.IsHitTestVisible = true;
			DgResult.IsHitTestVisible = true;
		}
		

		#region 事件处理

		private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.ResultCallBack = newResult.Select(fileModel => new FileViewModel(fileModel)).ToObservableCollection();

		private void FileRemoved(FileViewModel removedItem) => _ = _vm.FileViewModels.Remove(removedItem);
		private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => sender.Text = Regex.Replace(sender.Text, $@"[{FileX.GetInvalidNameChars} ]+", "");

		private void QuerySubmitted(AutoSuggestBox autoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs e) => _vm.FileViewModels = autoSuggestBox.Text is "" ? _vm.ResultCallBack : RelationsDataTable.FuzzySearchName(autoSuggestBox.Text, _vm.ResultCallBack);

		private void FileEditTagsRaised(FileViewModel fileViewModel) => App.Window.NavigateFrame.Content = new FileEditTagsPage(fileViewModel);
		
		private void OpenCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).Tag).FullName.Open();
		private void OpenExplorerCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).Tag).Path.Open();
		private async void RemoveCmClick(object sender, RoutedEventArgs e)
		{
			if (!await MessageDialogX.Warning("是否从软件移除该文件？")) return;
			if (!App.TryRemoveFileModel((FileViewModel)((FrameworkElement)sender).Tag)) return;
			_ = _vm.FileViewModels.Remove((FileViewModel)((FrameworkElement)sender).Tag);
		}
		private void PropertiesCmClick(object sender, RoutedEventArgs e) => PropertiesCmDoubleClick(((MenuFlyoutItem)sender).Tag, e);

		private void PropertiesCmDoubleClick(object sender, RoutedEventArgs e)
		{
			FileProperties.Load((FileViewModel)((FrameworkElement)sender).Tag);
			_ = FileProperties.ShowAsync(ContentDialogPlacement.Popup);
		}

		#endregion

	}
}