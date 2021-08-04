using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using static TagsTree.Properties.Settings;

namespace TagsTree.Views
{
	/// <summary>
	/// NewConfig.xaml 的交互逻辑
	/// </summary>
	public partial class NewConfig : Window
	{
		public NewConfig(Window? owner = null)
		{
			Owner = owner;
			InitializeComponent();
			MouseLeftButtonDown += (_, _) => DragMove();
			TbConfigPath.Text = Default.ConfigPath;
			TbLibraryPath.Text = Default.LibraryPath;
			CbRootFoldersExist.IsChecked = Default.PathTags;
		}

		private void BConfigPath_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new CommonOpenFileDialog("「配置路径」：选择存放配置文件的文件夹") { IsFolderPicker = true };
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
				TbConfigPath.Text = dialog.FileName;
		}

		private void BLibraryPath_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new CommonOpenFileDialog("「文件路径」：选择被归类文件的根目录文件夹") { IsFolderPicker = true };
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
				TbLibraryPath.Text = dialog.FileName;
		}

		private void BConfirm_Click(object sender, RoutedEventArgs e)
		{
			var legalPath = new Regex($@"^[a-zA-Z]:\\[^{App.FileX.GetInvalidPathChars}]+$");
			if (!legalPath.IsMatch(TbConfigPath.Text) || !legalPath.IsMatch(TbLibraryPath.Text))
				App.MessageBoxX.Error("路径错误！请填写正确完整的文件夹路径！");
			else
			{
				if (new DirectoryInfo(TbConfigPath.Text).GetFiles().Length != 0)
					App.MessageBoxX.Information("请保证存放配置文件的文件夹里没有重要的文件，防止受到损坏");
				if (Owner is not null && Default.ConfigPath != TbConfigPath.Text)
					switch (App.MessageBoxX.Question("是否将原位置配置文件移动到新位置"))
					{
						case true:
							foreach (var file in new DirectoryInfo(Default.ConfigPath).GetFiles())
								file.MoveTo(Path.Combine(TbConfigPath.Text, file.Name));
							break;
						case null: return;
					}
				Default.ConfigPath = TbConfigPath.Text;
				Default.LibraryPath = TbLibraryPath.Text;
				Default.PathTags = CbRootFoldersExist.IsChecked!.Value;
				Default.IsSet = true;
				Default.Save();
				DialogResult = true;
				Close();
			}
		}
	}
}