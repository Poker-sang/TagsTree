using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Views;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        TbLibraryPath.Text = App.AppConfiguration.LibraryPath;
        RbTheme.SelectedIndex = App.AppConfiguration.Theme;
        TsFilesObserver.IsOn = App.AppConfiguration.FilesObserverEnabled;
        CbRootFoldersExist.IsOn = App.AppConfiguration.PathTagsEnabled;
    }

    private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        var selectedTheme = (int)((FrameworkElement)sender).Tag switch
        {
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        ThemeHelper.RootTheme = selectedTheme;
        Application.Current.Resources["WindowCaptionForeground"] = selectedTheme switch
        {
            ElementTheme.Dark => Colors.White,
            ElementTheme.Light => Colors.Black,
            _ => Application.Current.RequestedTheme is ApplicationTheme.Dark ? Colors.White : Colors.Black
        };

        TitleBarHelper.TriggerTitleBarRepaint();
    }

    private async void BLibraryPath_Click(object sender, RoutedEventArgs e)
        => TbLibraryPath.Text = (await FileSystemHelper.GetStorageFolder())?.Path ?? TbLibraryPath.Text;

    private async void BExport_Click(object sender, RoutedEventArgs e)
    {
        if (await FileSystemHelper.GetStorageFolder() is { } folder)
            AppContext.AppLocalFolder.Copy(folder.Path);
    }

    private async void BImport_Click(object sender, RoutedEventArgs e)
    {
        if (await FileSystemHelper.GetStorageFolder() is { } folder)
            folder.Path.Copy(AppContext.AppLocalFolder);
    }
    private void BOpenDirectory_Click(object sender, RoutedEventArgs e) => AppContext.AppLocalFolder.Open();

    private async void BSave_Click(object sender, RoutedEventArgs e)
    {
        var legalPath = new Regex($@"^[a-zA-Z]:\\[^{FileSystemHelper.GetInvalidPathChars}]*$");
        if (!legalPath.IsMatch(TbLibraryPath.Text))
        {
            InfoBar.Severity = InfoBarSeverity.Error;
            InfoBar.Message = "错误";
            InfoBar.Message = "路径错误！请填写正确完整的文件夹路径！";
        }
        else
        {
            App.AppConfiguration.LibraryPath = TbLibraryPath.Text;
            App.AppConfiguration.PathTagsEnabled = CbRootFoldersExist.IsOn;
            App.AppConfiguration.FilesObserverEnabled = TsFilesObserver.IsOn;
            AppContext.SaveConfiguration(App.AppConfiguration);
            InfoBar.Severity = InfoBarSeverity.Success;
            InfoBar.Title = "成功";
            InfoBar.Message = "已保存";
            if (!App.ConfigSet)
            {
                App.ConfigSet = true;
                await WindowHelper.Window.ConfigIsSet();
            }
            else
                ((NavigationViewItem)App.RootNavigationView.FooterMenuItems[0]).IsEnabled = await App.ChangeFilesObserver();
        }

        InfoBar.IsOpen = true;
    }
}
