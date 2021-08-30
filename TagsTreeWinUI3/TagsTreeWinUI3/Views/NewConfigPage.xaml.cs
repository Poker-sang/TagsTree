using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Services;

namespace TagsTreeWinUI3.Views
{
	/// <summary>
	/// NewConfigPage.xaml 的交互逻辑
	/// </summary>
	public partial class NewConfigPage : Page
	{
		public NewConfigPage()
		{
			InitializeComponent();
			if (!App.ConfigSet)
			{
				TbProxyPath.Text = TbLibraryPath.Text = "";
				TsTheme.IsOn = false;
				CbRootFoldersExist.IsChecked = true;
			}
			else
			{
				TbProxyPath.Text = App.AppConfigurations.ProxyPath;
				TbLibraryPath.Text = App.AppConfigurations.LibraryPath;
				TsTheme.IsOn = App.AppConfigurations.Theme;
				CbRootFoldersExist.IsChecked = App.AppConfigurations.PathTags;
			}
		}
		private async void BConfigPath_Click(object sender, RoutedEventArgs e) => TbProxyPath.Text = (await FileX.GetStorageFolder())?.Path ?? TbProxyPath.Text;

		private async void BLibraryPath_Click(object sender, RoutedEventArgs e) => TbLibraryPath.Text = (await FileX.GetStorageFolder())?.Path ?? TbLibraryPath.Text;

		private async void BConfirm_Click(object sender, RoutedEventArgs e)
		{
			var legalPath = new Regex($@"^[a-zA-Z]:\\[^{FileX.GetInvalidPathChars}]*$");
			if (!legalPath.IsMatch(TbProxyPath.Text) || !legalPath.IsMatch(TbLibraryPath.Text))
				MessageDialogX.Information(true, "路径错误！请填写正确完整的文件夹路径！");
			else
			{
				if (App.AppConfigurations.ProxyPath != TbProxyPath.Text && App.AppConfigurations.ProxyPath is not "")
					switch (await MessageDialogX.Question("是否将原位置配置文件移动到新位置"))
					{
						case true:
							foreach (var file in new DirectoryInfo(App.AppConfigurations.ProxyPath).GetFiles())
								file.MoveTo(Path.Combine(TbProxyPath.Text, file.Name));
							break;
						case null: return;
					}

				App.AppConfigurations.ProxyPath = TbProxyPath.Text;
				App.AppConfigurations.LibraryPath = TbLibraryPath.Text;
				App.AppConfigurations.PathTags = CbRootFoldersExist.IsChecked!.Value;
				App.AppConfigurations.Theme = TsTheme.IsOn;
				AppConfigurations.SaveConfiguration(App.AppConfigurations);
				MessageDialogX.Information(false, "已保存");
				App.ConfigSet = true;
				App.Window.ConfigModeUnlock();
			}
		}

	}
}