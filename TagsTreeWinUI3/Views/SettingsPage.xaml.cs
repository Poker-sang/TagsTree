using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Services;

namespace TagsTreeWinUI3.Views
{
	/// <summary>
	/// SettingsPage.xaml 的交互逻辑
	/// </summary>
	public partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			InitializeComponent();
			TbLibraryPath.Text = App.AppConfigurations.LibraryPath;
			TsTheme.IsOn = App.AppConfigurations.Theme;
			TsFilesObserver.IsOn = App.AppConfigurations.FilesObserverEnabled;
			CbRootFoldersExist.IsOn = App.AppConfigurations.PathTagsEnabled;
		}

		private async void BLibraryPath_Click(object sender, RoutedEventArgs e) => TbLibraryPath.Text = (await FileX.GetStorageFolder())?.Path ?? TbLibraryPath.Text;

		private void BConfirm_Click(object sender, RoutedEventArgs e)
		{
			var legalPath = new Regex($@"^[a-zA-Z]:\\[^{FileX.GetInvalidPathChars}]*$");
			if (!legalPath.IsMatch(TbLibraryPath.Text))
				MessageDialogX.Information(true, "路径错误！请填写正确完整的文件夹路径！");
			else
			{
				App.AppConfigurations.LibraryPath = TbLibraryPath.Text;
				App.AppConfigurations.PathTagsEnabled = CbRootFoldersExist.IsOn;
				App.AppConfigurations.Theme = TsTheme.IsOn;
				App.AppConfigurations.FilesObserverEnabled = TsFilesObserver.IsOn;
				AppConfigurations.SaveConfiguration(App.AppConfigurations);
				MessageDialogX.Information(false, "已保存");
				App.ConfigSet = true;
				App.Window.ConfigModeUnlock();
			}
		}

	}
}