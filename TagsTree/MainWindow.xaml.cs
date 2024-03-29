using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Services;
using TagsTree.Views;
using WinUI3Utilities;

namespace TagsTree;

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
        CurrentContext.Frame = NavigationView.Content.To<Frame>();
    }

    private async void Loaded(object sender, RoutedEventArgs e)
    {
        NavigationView.SettingsItem.To<NavigationViewItem>().Tag = SettingsPage.TypeGetter;

        if (AppContext.SettingViewModel.ConfigSet)
            await ConfigIsSet();
        else
            DisplaySettings();
    }

    //    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    //    {
    //        DragZoneHelper.SetDragZones(new()
    //        {
    //#if DEBUG
    //            ExcludeDebugToolbarArea = true,
    //#endif
    //            DragZoneLeftIndent = (int)NavigationView.CompactPaneLength
    //        });
    //    }

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
        CurrentContext.Frame.GoBack();
        NavigationView.SelectedItem = CurrentContext.Frame.Content switch
        {
            IndexPage or
                TagSearchFilesPage or
                FilePropertiesPage or
                FileEditTagsPage => NavigationView.MenuItems[0],
            TagsManagerPage => NavigationView.MenuItems[1],
            FileImporterPage => NavigationView.MenuItems[2],
            SelectTagToEditPage or
                TagEditFilesPage => NavigationView.MenuItems[3],
            FilesObserverPage => NavigationView.FooterMenuItems[0],
            SettingsPage => NavigationView.SettingsItem,
            _ => NavigationView.SelectedItem
        };
        NavigationView.IsBackEnabled = CurrentContext.Frame.CanGoBack;
    }

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer.Tag is Type item && item != CurrentContext.Frame.Content.GetType())
            NavigationHelper.GotoPage(item);
    }

    private void TeachingTipOnLoaded(object sender, RoutedEventArgs e) => SnackBarHelper.RootSnackBar = sender.To<TeachingTip>();
}
