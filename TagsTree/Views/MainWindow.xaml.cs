using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ModernWpf.Controls;
using TagsTree.ViewModels;
using static TagsTree.Properties.Settings;

namespace TagsTree.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			while (true)
			{
				if (Default.IsSet) //如果之前有储存过用户配置，则判断是否符合
				{
					var legalPath = new Regex(@"^[a-zA-Z]:\\[^\/\:\*\?\""\<\>\|\,]+$");
					if (legalPath.IsMatch(Default.ConfigPath) && legalPath.IsMatch(Default.LibraryPath))
					{
						if (App.LoadConfig(Default.ConfigPath))
							break;
					}
					else
					{
						App.ErrorMessageBox("配置文件损坏！请重新输入");
						Default.IsSet = false;
					}
				}
				else if (new NewConfig(this).ShowDialog() == false)
				{
					Close();
					return;
				}
			}
			var vm= Services.MainWindowService.Load(this);
			DataContext = vm;
			MouseLeftButtonDown += (_, _) => DragMove();
			InitializeComponent();

			TbInput.SuggestionChosen += vm.SuggestionChosen;
			TbInput.TextChanged += vm.TextChanged;
			TbInput.QuerySubmitted += TbInput_OnQuerySubmitted + vm.QuerySubmitted;
		}
		
		private static async void Search(IAnimatable textBlock, IAnimatable textBox, UIElement dataGrid)
		{
			textBlock.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 0, 0, 0),
				To = new Thickness(0, -350, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			textBox.BeginAnimation(MarginProperty, new ThicknessAnimation
			{
				From = new Thickness(0, 300, 0, 0),
				To = new Thickness(0, 80, 0, 0),
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			await Task.Delay(1000);
			dataGrid.BeginAnimation(OpacityProperty, new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds(1000)
			});
			dataGrid.IsHitTestVisible = true;
		}

		private bool _isSearched;
		private void TbInput_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (_isSearched) return;
			Search(TbBanner, TbInput, DgResult);
			_isSearched = true;
		}


		private void ChangeConfig_Click(object sender, RoutedEventArgs e) => _ = new NewConfig(this).ShowDialog();
		private void TagsManager_Click(object sender, RoutedEventArgs e) => _ = new TagsManager(this).ShowDialog();
		private void FileAdder_OnClick(object sender, RoutedEventArgs e) => _ = new FileImporter(this).ShowDialog();

	}
}