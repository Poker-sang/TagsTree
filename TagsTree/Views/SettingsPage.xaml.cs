using System;
using System.IO;
using Windows.System;
using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Interfaces;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

[INotifyPropertyChanged]
public partial class SettingsPage : Page, ITypeGetter
{
    public SettingsPage()
    {
        Current = this;
        InitializeComponent();
    }

    public static Type TypeGetter => typeof(SettingsPage);

    public static SettingsPage Current { get; private set; } = null!;

#pragma warning disable CA1822
    private SettingsViewModel Vm => AppContext.SettingViewModel;
#pragma warning restore CA1822

    #region 事件处理

    private void NavigateUriClicked(object sender, RoutedEventArgs e)
    {
        Launcher.LaunchUriAsync(new(sender.To<FrameworkElement>().GetTag<string>()));
    }

    private void ThemeChecked(object sender, RoutedEventArgs e)
    {
        var selectedTheme = sender.To<FrameworkElement>().GetTag<int>() switch
        {
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        if (App.MainWindow.Content is FrameworkElement rootElement)
            rootElement.RequestedTheme = selectedTheme;

        AppContext.AppConfig.Theme = selectedTheme.To<int>();
    }

    private async void LibraryPathClicked(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        => sender.Text = (await App.MainWindow.PickSingleFolderAsync())?.Path;

    private async void ExportClicked(object sender, RoutedEventArgs e)
    {
        if (await App.MainWindow.PickSingleFolderAsync() is { } folder)
            AppContext.AppLocalFolder.Copy(folder.Path);
    }

    private async void ImportClicked(object sender, RoutedEventArgs e)
    {
        if (await App.MainWindow.PickSingleFolderAsync() is { } folder)
            folder.Path.Copy(AppContext.AppLocalFolder);
    }

    private void OpenDirectoryClicked(object sender, RoutedEventArgs e) => AppContext.AppLocalFolder.Open();

    private async void LibraryPathSaved(object sender, RoutedEventArgs e)
    {
        var asb = sender.To<SettingsCard>().Description.To<AutoSuggestBox>();
        if (!Directory.Exists(asb.Text))
        {
            asb.Description = "路径错误！请填写正确、完整、存在的文件夹路径！";
            return;
        }

        asb.Description = "";

        Vm.LibraryPath = asb.Text;
        asb.Text = "";
        Vm.ConfigSetChanged();

        await App.MainWindow.ConfigIsSet();
    }

    private void SetDefaultAppConfigClicked(object sender, RoutedEventArgs e)
    {
        AppContext.SetDefaultAppConfig();
        OnPropertyChanged(nameof(Vm));
    }

    private new void Unloaded(object sender, RoutedEventArgs e) => AppContext.SaveConfiguration(Vm.AppConfig);

    #endregion
}
