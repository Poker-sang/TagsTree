using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TagsTreeWpf.Delegates;
using TagsTreeWpf.Models;
using TagsTreeWpf.Services;
using TagsTreeWpf.Services.ExtensionMethods;
using TagsTreeWpf.ViewModels;
using TagsTreeWpf.Views.Controls;
using static TagsTreeWpf.Properties.Settings;

namespace TagsTreeWpf.Views
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		public Main()
		{
			DataContext = _vm = new MainViewModel();
			InitializeComponent();
			if (!CheckConfig())
			{
				Close();
				return;
			}

			TbSearch.BeforeQuerySubmitted = (_, _) => Search();
			_ = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => Keyboard.Focus(TbSearch)));
		}

		private readonly MainViewModel _vm;

		private bool _isSearched;
		private async void Search()
		{
			if (_isSearched) return;
			_isSearched = true;
			TbBanner.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 0, 0, 0),
				To = new Thickness(0, -350, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			TbSearch.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 300, 0, 0),
				To = new Thickness(0, 50, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			await Task.Delay(1000);
			TbFuzzySearch.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			DgResult.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			TbFuzzySearch.IsHitTestVisible = true;
			DgResult.IsHitTestVisible = true;
		}

		private void TagsManager_Click(object sender, RoutedEventArgs e) => _ = new TagsManager(this).ShowDialog();
		private void FileAdder_OnClick(object sender, RoutedEventArgs e) => _ = new FileImporter(this).ShowDialog();
		private void TagEditFiles_OnClick(object sender, RoutedEventArgs e) => _ = new TagEditFiles(this).ShowDialog();
		private void ChangeConfig_Click(object sender, RoutedEventArgs e)
		{
			if (!MessageBoxX.Warning("更改设置后需要重启软件，请确保已保存")) return;
			if (new NewConfig(this).ShowDialog() == true) Close();
		}


		private bool _isShowed;

		#region 事件处理

		private void MainMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!_isShowed || e.GetPosition((Main)sender).X is >= 215 and <= 1065) return;
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
		private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			_vm.FileViewModels = sender.Text is "" ? _vm.ResultCallBack : RelationsDataTable.FuzzySearchName(sender.Text, _vm.ResultCallBack);
			_vm.CollectionChanged();
		}

		#endregion

		#region 命令

		private void OpenCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((DataGridRow)((MenuItem)sender).Tag).DataContext).FullName.Open();
		private void OpenExplorerCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((DataGridRow)((MenuItem)sender).Tag).DataContext).Path.Open();
		private void RemoveCmClick(object sender, RoutedEventArgs e)
		{
			if (!MessageBoxX.Warning("是否从软件移除该文件？")) return;
			if (!App.TryRemoveFileModel((FileViewModel)((DataGridRow)((MenuItem)sender).Tag).DataContext)) return;
			_ = _vm.FileViewModels.Remove((FileViewModel)((DataGridRow)((MenuItem)sender).Tag).DataContext);
			_vm.CollectionChanged();
		}
		private void PropertiesCmClick(object sender, RoutedEventArgs e) => PropertiesCmDoubleClick(((MenuItem)sender).Tag, e);

		private async void PropertiesCmDoubleClick(object sender, RoutedEventArgs e)
		{
			FileProperties.Load(this, (FileViewModel)((DataGridRow)sender).DataContext);
			_ = FileProperties.ShowAsync(ContentDialogPlacement.Popup);
			await Task.Delay(100);
			_isShowed = true;
		}

		#endregion

		#region 操作

		private static bool CheckConfig()
		{
			ThemeManager.Current.ApplicationTheme = Default.Theme ? ApplicationTheme.Dark : ApplicationTheme.Light;
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