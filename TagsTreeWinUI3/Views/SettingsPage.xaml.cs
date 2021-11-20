using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;

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
            CbTheme.SelectedIndex = App.AppConfigurations.Theme;
            TsFilesObserver.IsOn = App.AppConfigurations.FilesObserverEnabled;
            CbRootFoldersExist.IsOn = App.AppConfigurations.PathTagsEnabled;
        }

        private async void BLibraryPath_Click(object sender, RoutedEventArgs e)
        {
            TbLibraryPath.Text = (await FileSystemHelper.GetStorageFolder())?.Path ?? TbLibraryPath.Text;
        }

        private async void BExport_Click(object sender, RoutedEventArgs e)
        {
            if (await FileSystemHelper.GetStorageFolder() is { } folder)
                AppConfigurations.AppLocalFolder.Copy(folder.Path);
        }

        private async void BImport_Click(object sender, RoutedEventArgs e)
        {
            if (await FileSystemHelper.GetStorageFolder() is { } folder)
                folder.Path.Copy(AppConfigurations.AppLocalFolder);
        }
        private void BOpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            AppConfigurations.AppLocalFolder.Open();
        }

        private async void BSave_Click(object sender, RoutedEventArgs e)
        {
            var legalPath = new Regex($@"^[a-zA-Z]:\\[^{FileSystemHelper.GetInvalidPathChars}]*$");
            if (!legalPath.IsMatch(TbLibraryPath.Text))
                await ShowMessageDialog.Information(true, "路径错误！请填写正确完整的文件夹路径！");
            else
            {
                App.AppConfigurations.LibraryPath = TbLibraryPath.Text;
                App.AppConfigurations.PathTagsEnabled = CbRootFoldersExist.IsOn;
                App.AppConfigurations.Theme = CbTheme.SelectedIndex;
                App.RootNavigationView.RequestedTheme = CbTheme.SelectedIndex switch
                {
                    1 => ElementTheme.Light,
                    2 => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };
                App.AppConfigurations.FilesObserverEnabled = TsFilesObserver.IsOn;
                AppConfigurations.SaveConfiguration(App.AppConfigurations);
                await ShowMessageDialog.Information(false, "已保存");
                if (!App.ConfigSet)
                {
                    App.ConfigSet = true;
                    await App.Window.ConfigIsSet();
                }

                ((NavigationViewItem)App.RootNavigationView.FooterMenuItems[0]).IsEnabled = await App.ChangeFilesObserver(); //就是App.AppConfigurations.FilesObserverEnabled;
            }
        }
    }
}