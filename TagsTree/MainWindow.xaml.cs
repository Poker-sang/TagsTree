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
    public MainWindow() => InitializeComponent();

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
            GotoPage<IndexPage>();
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        IconsHelper.LoadFilesIcons();

        foreach (var menuItem in NavigationView.MenuItems.Cast<NavigationViewItem>())
            menuItem.IsEnabled = true;

        await AppContext.FilesObserverChanged();
    }

    public void GotoPage<T>(object? parameter = null) where T : Page => Frame.Navigate(typeof(T), parameter);

    private void DisplaySettings()
    {
        GotoPage<SettingsPage>();
        NavigationView.SelectedItem = NavigationView.SettingsItem;
    }

    private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        Frame.GoBack();
        NavigationView.SelectedItem = Frame.Content switch
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
        NavigationView.IsBackEnabled = Frame.CanGoBack;
    }

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer.Tag is Type item && item != Frame.Content.GetType())
            _ = Frame.Navigate(item);
    }

    private void OnPaneChanging(NavigationView sender, object e)
    {
        sender.UpdateAppTitleMargin(TitleTextBlock);
    }
}
