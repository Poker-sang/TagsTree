using System.IO;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.System;
using Microsoft.UI.Xaml.Input;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Views;

public partial class SettingsPage : Page
{
    public SettingsPage() => InitializeComponent();

    private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        var selectedTheme = (int)((FrameworkElement)sender).Tag switch
        {
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        // 内含 App.AppConfiguration.Theme = selectedTheme;
        ThemeHelper.RootTheme = selectedTheme;
        Application.Current.Resources["WindowCaptionForeground"] = selectedTheme switch
        {
            ElementTheme.Dark => Colors.White,
            ElementTheme.Light => Colors.Black,
            _ => Application.Current.RequestedTheme is ApplicationTheme.Dark ? Colors.White : Colors.Black
        };

        TitleBarHelper.TriggerTitleBarRepaint();
        AppContext.SaveConfiguration(App.AppConfiguration);
    }

    private async void BLibraryPath_Click(object sender, RoutedEventArgs e)
    {
        TbLibraryPath.Text = (await FileSystemHelper.GetStorageFolder())?.Path ?? TbLibraryPath.Text;
        await ValidLibraryPathSet(TbLibraryPath.Text);
    }

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

    private void FilesObserver_OnToggled(object sender, RoutedEventArgs e)
    {
        App.AppConfiguration.FilesObserverEnabled = ((ToggleSwitch)sender).IsOn;
        AppContext.SaveConfiguration(App.AppConfiguration);
    }

    private void PathTagsEnabled_OnToggled(object sender, RoutedEventArgs e)
    {
        App.AppConfiguration.PathTagsEnabled = ((ToggleSwitch)sender).IsOn;
        AppContext.SaveConfiguration(App.AppConfiguration);
    }

    private async void TbLibraryPath_OnCharacterReceived(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is not VirtualKey.Enter) 
            return;
        if (Directory.Exists(((TextBox)sender).Text))
        {
            await ValidLibraryPathSet(((TextBox)sender).Text);
            ((TextBox)sender).Description = "";
        }
        else
            ((TextBox)sender).Description = "路径错误！请填写正确、完整、存在的文件夹路径！";
    }

    private static async Task ValidLibraryPathSet(string path)
    {
        App.AppConfiguration.LibraryPath = path;
        AppContext.SaveConfiguration(App.AppConfiguration);
        if (!App.ConfigSet)
        {
            App.ConfigSet = true;
            await WindowHelper.Window.ConfigIsSet();
        }
        else
            ((NavigationViewItem)App.RootNavigationView.FooterMenuItems[0]).IsEnabled = await App.ChangeFilesObserver();
    }
}
