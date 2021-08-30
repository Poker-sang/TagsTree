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
				LoadConfig();
				App.Window.ConfigModeUnlock();
			}
		}
		///  <summary>
		///  重新加载新的配置文件
		///  </summary>
		///  <returns>true：已填写正确地址，进入软件；false：打开设置页面；null：关闭软件</returns>
		private async void LoadConfig()
		{
			//标签
			App.Tags.LoadTree(App.TagsPath);
			App.Tags.LoadDictionary();

			//文件
			App.IdFile.Deserialize(App.FilesPath);

			//关系
			if (!File.Exists(App.RelationsPath))
				_ = File.Create(App.RelationsPath);
			App.Relations.Load(); //异常在内部处理

			//检查
			if (App.Tags.TagsDictionary.Count != App.Relations.Columns.Count) //TagsDictionary第一个是总根标签，Relations第一列是文件Id 
			{
				if (await MessageDialogX.Warning($"路径「{App.AppConfigurations.ProxyPath}」下，TagsTree.xml和Relations.xml存储的标签数不同", "删除关系文件Relations.xml并重新生成", "直接关闭软件"))
				{
					File.Delete(App.RelationsPath);
					App.Relations.Load();
				}
				else
				{
					App.Window.Close();
					return;
				}
			}
			if (App.IdFile.Count != App.Relations.Rows.Count)
			{
				if (await MessageDialogX.Warning($"「路径{App.AppConfigurations.ProxyPath}」下，Files.json和Relations.xml存储的文件数不同", "删除关系文件Relations.xml并重新生成", "直接关闭软件"))
				{
					File.Delete(App.RelationsPath);
					App.Relations.Load();
				}
				else
				{
					App.Window.Close();
					return;
				}
			}
		}
	}
}