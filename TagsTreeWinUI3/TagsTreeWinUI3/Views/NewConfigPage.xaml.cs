using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using System.Text.RegularExpressions;
using Windows.Storage.Pickers;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;

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
		}


		private async void BConfigPath_Click(object sender, RoutedEventArgs e) => TbConfigPath.Text = (await new FolderPicker().InitializeWithWindow().PickSingleFolderAsync())?.Path ?? TbConfigPath.Text;
		private async void BLibraryPath_Click(object sender, RoutedEventArgs e) => TbLibraryPath.Text = (await new FolderPicker().InitializeWithWindow().PickSingleFolderAsync())?.Path ?? TbLibraryPath.Text;

		private void TgTheme_OnToggled(object sender, RoutedEventArgs e)
		{
			;
		}

		private async void BConfirm_Click(object sender, RoutedEventArgs e)
		{
			var legalPath = new Regex($@"^[a-zA-Z]:\\[^{FileX.GetInvalidPathChars}]*$");
			if (!legalPath.IsMatch(TbConfigPath.Text) || !legalPath.IsMatch(TbLibraryPath.Text))
				MessageDialogX.Information(true, "路径错误！请填写正确完整的文件夹路径！");
			else
			{
				if (App.AppConfigurations is not null && App.AppConfigurations.ProxyPath != TbConfigPath.Text)
					switch (await MessageDialogX.Question("是否将原位置配置文件移动到新位置"))
					{
						case true:
							foreach (var file in new DirectoryInfo(App.AppConfigurations.ProxyPath).GetFiles())
								file.MoveTo(Path.Combine(TbConfigPath.Text, file.Name));
							break;
						case null: return;
					}

				App.AppConfigurations.ProxyPath = TbConfigPath.Text;
				App.AppConfigurations.LibraryPath = TbLibraryPath.Text;
				App.AppConfigurations.PathTags = CbRootFoldersExist.IsChecked!.Value;
				//App.AppConfigurations.Theme = TgTheme.IsOn;
				AppConfigurations.SaveConfiguration(App.AppConfigurations);
				MessageDialogX.Information(false, "已保存");
			}
		}

	}
}