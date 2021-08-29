using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PInvoke;
using TagsTreeWinUI3.Commands;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.Views;

namespace TagsTreeWinUI3
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			SetTitleBar(TitleBar);
		}

		private void NavigationView_OnLoaded(object sender, RoutedEventArgs e) => NavigateFrame.Content = new NewConfigPage();

		public async void ConfigModeUnlock()
		{
			foreach (NavigationViewItem menuItem in NavigationView.MenuItems)
				menuItem.IsEnabled = true;
			await Task.Delay(500);
			NavigationView.SelectedItem = NavigationView.MenuItems[0];
		}
		//不为static方便绑定
		private Brush SystemColor => new SolidColorBrush(Application.Current.RequestedTheme is ApplicationTheme.Light ? Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF):Color.FromArgb(0x65, 0x00, 0x00, 0x00));

		private void Selector_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs e)
		{
			if (e.SelectedItem == sender.MenuItems[0])
				NavigateFrame.Content = new IndexPage();
			else if (e.SelectedItem == sender.MenuItems[1])
				NavigateFrame.Content = new TagsManagerPage();
			else if (e.SelectedItem == sender.MenuItems[2])
				NavigateFrame.Content = new FileImporterPage();
			else if (e.SelectedItem == sender.MenuItems[3])
				NavigateFrame.Content = new TagEditFilesPage();
			else if (e.IsSettingsSelected)
				NavigateFrame.Content = new NewConfigPage();
			GC.Collect();
		}
	}
}