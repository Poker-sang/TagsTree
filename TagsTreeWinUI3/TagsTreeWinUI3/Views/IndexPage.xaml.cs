using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Linq;
using System.Reflection;
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

			TbSearch.BeforeQuerySubmitted = (_, _) => Search();
			//_ = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => Keyboard.Focus(TbSearch)));
		}

		private readonly MainViewModel _vm;

		private bool _isSearched;
		private void Search()
		{
			//if (_isSearched) return;
			//_isSearched = true;
			//TbBanner.BeginAnimation(MarginProperty, new ThicknessAnimation
			//{
			//	From = new Thickness(0, 0, 0, 0),
			//	To = new Thickness(0, -350, 0, 0),
			//	Duration = TimeSpan.FromMilliseconds(1000)
			//});
			//TbSearch.BeginAnimation(MarginProperty, new ThicknessAnimation
			//{
			//	From = new Thickness(0, 300, 0, 0),
			//	To = new Thickness(0, 50, 0, 0),
			//	Duration = TimeSpan.FromMilliseconds(1000)
			//});
			//await Task.Delay(1000);
			//TbFuzzySearch.BeginAnimation(OpacityProperty, new DoubleAnimation
			//{
			//	From = 0,
			//	To = 1,
			//	Duration = TimeSpan.FromMilliseconds(1000)
			//});
			//DgResult.BeginAnimation(OpacityProperty, new DoubleAnimation
			//{
			//	From = 0,
			//	To = 1,
			//	Duration = TimeSpan.FromMilliseconds(1000)
			//});
			//TbFuzzySearch.IsHitTestVisible = true;
			//DgResult.IsHitTestVisible = true;
		}

		private bool _isShowed;

		#region 事件处理

		private void GridMouseLeftButtonDown(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
		{
			if (!_isShowed || pointerRoutedEventArgs.GetCurrentPoint((Grid)sender).Properties.IsLeftButtonPressed && pointerRoutedEventArgs.GetCurrentPoint((Grid)sender).Position.X is >= 215 and <= 1065) return;
			FileProperties.Hide();
			_isShowed = false;
		}


		private void ResultChanged(TagSearchBox sender, ResultChangedEventArgs e) => _vm.ResultCallBack = e.NewResult.Select(fileModel => new FileViewModel(fileModel)).ToObservableCollection();

		private void FileRemoved(FileProperties sender, FileRemovedEventArgs e)
		{
			_ = _vm.FileViewModels.Remove(e.RemovedItem);
			_vm.CollectionChanged();
		}
		private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			sender.Text = Regex.Replace(sender.Text, $@"[{FileX.GetInvalidNameChars} ]+", "");
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}
		private void QuerySubmitted(AutoSuggestBox autoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			_vm.FileViewModels = autoSuggestBox.Text is "" ? _vm.ResultCallBack : RelationsDataTable.FuzzySearchName(autoSuggestBox.Text, _vm.ResultCallBack);
			_vm.CollectionChanged();
		}

		#endregion

		#region 命令

		private void OpenCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).Tag).FullName.Open();
		private void OpenExplorerCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).Tag).Path.Open();
		private async void RemoveCmClick(object sender, RoutedEventArgs e)
		{
			if (!await MessageDialogX.Warning("是否从软件移除该文件？")) return;
			if (!App.TryRemoveFileModel((FileViewModel)((FrameworkElement)sender).Tag)) return;
			_ = _vm.FileViewModels.Remove((FileViewModel)((FrameworkElement)sender).Tag);
			_vm.CollectionChanged();
		}
		private void PropertiesCmClick(object sender, RoutedEventArgs e) => PropertiesCmDoubleClick(((MenuFlyoutItem)sender).Tag, e);

		private async void PropertiesCmDoubleClick(object sender, RoutedEventArgs e)
		{
			FileProperties.Load((FileViewModel)((FrameworkElement)sender).Tag);
			_ = FileProperties.ShowAsync(ContentDialogPlacement.Popup);
			await Task.Delay(100);
			_isShowed = true;
		}

		#endregion
	}
}