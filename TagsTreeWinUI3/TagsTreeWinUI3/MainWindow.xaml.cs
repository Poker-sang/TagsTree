using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
		}

		public void ConfigModeUnlock()
		{
			NavigationView.SelectedItem = NavigationView.MenuItems[0];
			foreach (NavigationViewItem menuItem in NavigationView.MenuItems)
				menuItem.IsEnabled = true;
		}
		
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