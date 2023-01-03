using System.IO;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using TagsTree.Services.ExtensionMethods;
using Windows.System;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class SettingsPage : Page
{
    public SettingsPage() => InitializeComponent();

    private void ThemeChecked(object sender, RoutedEventArgs e)
    {
        var selectedTheme = sender.GetTag<int>() switch
        {
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        if (CurrentContext.Window.Content is FrameworkElement rootElement)
            rootElement.RequestedTheme = selectedTheme;

        CurrentContext.App.Resources["WindowCaptionForeground"] = selectedTheme switch
        {
            ElementTheme.Dark => Colors.White,
            ElementTheme.Light => Colors.Black,
            _ => CurrentContext.App.RequestedTheme is ApplicationTheme.Dark ? Colors.White : Colors.Black
        };

        App.AppConfig.Theme = (int)selectedTheme;

        AppContext.SaveConfiguration(App.AppConfig);
    }

    private async void LibraryPathClick(object sender, RoutedEventArgs e)
    {
        TbLibraryPath.Text = (await PickerHelper.PickSingleFolderAsync())?.Path ?? TbLibraryPath.Text;
        await ValidLibraryPathSet(TbLibraryPath.Text);
    }

    private async void ExportClick(object sender, RoutedEventArgs e)
    {
        if (await PickerHelper.PickSingleFolderAsync() is { } folder)
            AppContext.AppLocalFolder.Copy(folder.Path);
    }

    private async void ImportClick(object sender, RoutedEventArgs e)
    {
        if (await PickerHelper.PickSingleFolderAsync() is { } folder)
            folder.Path.Copy(AppContext.AppLocalFolder);
    }

    private void OpenDirectoryClick(object sender, RoutedEventArgs e) => AppContext.AppLocalFolder.Open();

    private void FilesObserver_OnToggled(object sender, RoutedEventArgs e)
    {
        App.AppConfig.FilesObserverEnabled = ((ToggleSwitch)sender).IsOn;
        AppContext.SaveConfiguration(App.AppConfig);
    }

    private void PathTagsEnabledToggled(object sender, RoutedEventArgs e)
    {
        App.AppConfig.PathTagsEnabled = ((ToggleSwitch)sender).IsOn;
        AppContext.SaveConfiguration(App.AppConfig);
    }

    private async void TbLibraryPathCharacterReceived(object sender, KeyRoutedEventArgs e)
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
        App.AppConfig.LibraryPath = path;
        AppContext.SaveConfiguration(App.AppConfig);
        if (!App.ConfigSet)
        {
            App.ConfigSet = true;
            await ((MainWindow)CurrentContext.Window).ConfigIsSet();
        }
        else
            ((NavigationViewItem)CurrentContext.NavigationView.FooterMenuItems[0]).IsEnabled = await App.ChangeFilesObserver();
    }
}
