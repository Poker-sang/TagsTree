using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;
using TagsTreeWinUI3.Views;
using Windows.UI;

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

		public bool SetFilesObserverEnable { set => ((NavigationViewItem)NavigationView.MenuItems[4]).IsEnabled = value; }

		private void NavigationView_OnLoaded(object sender, RoutedEventArgs e) => NavigateFrame.Content = new SettingsPage();

		public async void ConfigModeUnlock()
		{
			foreach (NavigationViewItem menuItem in NavigationView.MenuItems)
				menuItem.IsEnabled = true;
			await Task.Delay(500);
			NavigationView.SelectedItem = NavigationView.MenuItems[0];
		}
		/// <summary>
		///不为static方便绑定
		/// </summary>
		private Brush SystemColor => new SolidColorBrush(Application.Current.RequestedTheme is ApplicationTheme.Light ? Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF) : Color.FromArgb(0x65, 0x00, 0x00, 0x00));

		private void Selector_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs e)
		{
			if (e.SelectedItem == sender.MenuItems[0])
				NavigateFrame.Navigate(typeof(IndexPage));
			else if (e.SelectedItem == sender.MenuItems[1])
				NavigateFrame.Navigate(typeof(TagsManagerPage));
			else if (e.SelectedItem == sender.MenuItems[2])
				NavigateFrame.Navigate(typeof(FileImporterPage));
			else if (e.SelectedItem == sender.MenuItems[3])
				NavigateFrame.Navigate(typeof(TagEditFilesPage));
			else if (e.SelectedItem == sender.MenuItems[4])
				NavigateFrame.Navigate(typeof(FilesObserverPage));
			else if (e.IsSettingsSelected)
				NavigateFrame.Navigate(typeof(SettingsPage));
			GC.Collect();
		}
	}
}