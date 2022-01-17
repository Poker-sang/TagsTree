using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf;
using ModernWpf.Controls;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using TagsTreeWpf.Services;
using static TagsTreeWpf.Properties.Settings;

namespace TagsTreeWpf.Views
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

        private void TgTheme_OnToggled(object sender, RoutedEventArgs e) => ThemeManager.Current.ApplicationTheme = ((ToggleSwitch)sender).IsOn ? ApplicationTheme.Dark : ApplicationTheme.Light;

        private void BConfirm_Click(object sender, RoutedEventArgs e)
        {
            var legalPath = new Regex($@"^[a-zA-Z]:\\[^{FileX.GetInvalidPathChars}]*$");
            if (!legalPath.IsMatch(TbConfigPath.Text) || !legalPath.IsMatch(TbLibraryPath.Text))
                MessageBoxX.Error("路径错误！请填写正确完整的文件夹路径！");
            else
            {
                if (Owner is not null && Default.ConfigPath != TbConfigPath.Text)
                    switch (MessageBoxX.Question("是否将原位置配置文件移动到新位置"))
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
                Default.Theme = TgTheme.IsOn;
                Default.IsSet = true;
                Default.Save();
                DialogResult = true;
                Close();
            }
        }

    }
}