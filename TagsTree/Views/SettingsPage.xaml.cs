using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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

    private void NavigateUriTapped(object sender, TappedRoutedEventArgs e)
    {
        using var process = new Process
        {
            StartInfo = new()
            {
                FileName = sender.GetTag<string>(),
                UseShellExecute = true
            }
        };
        _ = process.Start();
    }

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

        AppContext.AppConfig.Theme = selectedTheme.To<int>();
    }

    private async void LibraryPathTapped(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        => sender.Text = (await PickerHelper.PickSingleFolderAsync())?.Path;

    private async void ExportTapped(object sender, TappedRoutedEventArgs e)
    {
        if (await PickerHelper.PickSingleFolderAsync() is { } folder)
            AppContext.AppLocalFolder.Copy(folder.Path);
    }

    private async void ImportTapped(object sender, TappedRoutedEventArgs e)
    {
        if (await PickerHelper.PickSingleFolderAsync() is { } folder)
            folder.Path.Copy(AppContext.AppLocalFolder);
    }

    private void OpenDirectoryTapped(object sender, TappedRoutedEventArgs e) => AppContext.AppLocalFolder.Open();

    private async void LibraryPathSaved(object sender, TappedRoutedEventArgs e)
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

        await CurrentContext.Window.To<MainWindow>().ConfigIsSet();
    }

    private void SetDefaultAppConfigTapped(object sender, TappedRoutedEventArgs e)
    {
        AppContext.SetDefaultAppConfig();
        OnPropertyChanged(nameof(Vm));
    }

    private new void Unloaded(object sender, RoutedEventArgs e) => AppContext.SaveConfiguration(Vm.AppConfig);

    #endregion
}
