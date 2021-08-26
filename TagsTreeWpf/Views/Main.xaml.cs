using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TagsTreeWpf.Services;
using static TagsTreeWpf.Properties.Settings;
using Frame = System.Windows.Controls.Frame;
using ListView = System.Windows.Controls.ListView;

namespace TagsTreeWpf.Views
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		public Main()
		{
			App.Win = this;
			InitializeComponent();
			ListView.SelectedIndex = 0;
			if (!CheckConfig()) Close();
		}
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

		private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0 && e.AddedItems[0] == e.RemovedItems[0]) return;
			switch (((ListView)sender).SelectedIndex)
			{
				case 0: Frame.Content = new IndexPage(); break;
				case 1: Frame.Content = new TagsManagerPage(); break;
				case 2: Frame.Content = new FileImporterPage(); break;
				case 3: Frame.Content = new TagEditFilesPage(); break;
				case 4:
					if (MessageBoxX.Warning("更改设置后需要重启软件，请确保已保存") && new NewConfig().ShowDialog() == true)
						Close();
					((ListView)sender).SelectedItem = e.RemovedItems[0];
					return;
			}
			GC.Collect();
		}

		private void Frame_OnNavigated(object sender, NavigationEventArgs e)
		{
			if (((Frame)sender).Content.GetType() == typeof(IndexPage))
				ListView.SelectedIndex = 0;
			else if (((Frame)sender).Content.GetType() == typeof(TagsManagerPage))
				ListView.SelectedIndex = 1;
			else if (((Frame)sender).Content.GetType() ==  typeof(FileImporterPage))
				ListView.SelectedIndex = 2;
			else if (((Frame)sender).Content.GetType() == typeof(TagEditFilesPage))
				ListView.SelectedIndex = 3;
		}
	}
}