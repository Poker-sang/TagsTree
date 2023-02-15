using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Services;
using TagsTree.Views;
using WinUI3Utilities;

namespace TagsTree;

[INotifyPropertyChanged]
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        CurrentContext.Window = this;
        InitializeComponent();
        CurrentContext.TitleBar = TitleBar;
        CurrentContext.TitleTextBlock = TitleTextBlock;
        // TODO: Microsoft.WindowsAppSDK 1.2后，最小化的NavigationView没有高度
        CurrentContext.NavigationView = NavigationView;
        CurrentContext.Frame = NavigateFrame;
    }

    private async void Loaded(object sender, RoutedEventArgs e)
    {
        NavigationView.SettingsItem.To<NavigationViewItem>().Tag = SettingsPage.TypeGetter;

        if (AppContext.SettingViewModel.ConfigSet)
            await ConfigIsSet();
        else
            DisplaySettings();
    }

    public async Task ConfigIsSet()
    {
        if (AppContext.LoadConfig() is { } exception)
        {
            DisplaySettings();
            await AppContext.ExceptionHandler(exception);
        }
        else
        {
            NavigationHelper.GotoPage<IndexPage>();
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        IconsHelper.LoadFilesIcons();

        foreach (var menuItem in NavigationView.MenuItems.Cast<NavigationViewItem>())
            menuItem.IsEnabled = true;

        await AppContext.FilesObserverChanged();
    }

    private void DisplaySettings()
    {
        NavigationHelper.GotoPage<SettingsPage>();
        NavigationView.SelectedItem = NavigationView.SettingsItem;
    }

    private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        NavigateFrame.GoBack();
        NavigationView.SelectedItem = NavigateFrame.Content switch
        {
            IndexPage => NavigationView.MenuItems[0],
            TagSearchFilesPage => NavigationView.MenuItems[0],
            FilePropertiesPage => NavigationView.MenuItems[0],
            FileEditTagsPage => NavigationView.MenuItems[0],
            TagsManagerPage => NavigationView.MenuItems[1],
            FileImporterPage => NavigationView.MenuItems[2],
            SelectTagToEditPage => NavigationView.MenuItems[3],
            TagEditFilesPage => NavigationView.MenuItems[3],
            FilesObserverPage => NavigationView.FooterMenuItems[0],
            SettingsPage => NavigationView.SettingsItem,
            _ => NavigationView.SelectedItem
        };
        NavigationView.IsBackEnabled = NavigateFrame.CanGoBack;
    }

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer.Tag is Type item && item != NavigateFrame.Content.GetType())
            NavigationHelper.GotoPage(item);
    }
}
