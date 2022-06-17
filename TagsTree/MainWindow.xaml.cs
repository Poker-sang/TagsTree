using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using TagsTree.Services;
using TagsTree.Views;
using Windows.Foundation.Metadata;

namespace TagsTree;

[INotifyPropertyChanged]
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // 加载窗口后设置标题，拖拽区域才能达到原定效果
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        //UpdateTitleBarColor();
        Title = "TagsTree";

        App.RootNavigationView = NavigationView;
        App.RootFrame = NavigateFrame;
    }

    private void UpdateTitleBarColor()
    {
        var res = Application.Current.Resources;
        res["WindowCaptionBackground"] = _currentBgColor;
        //res["WindowCaptionBackgroundDisabled"] = currentBgColor;
        res["WindowCaptionForeground"] = _currentFgColor;
        //res["WindowCaptionForegroundDisabled"] = currentFgColor;

        TitleBarHelper.TriggerTitleBarRepaint();
    }
    private readonly Windows.UI.Color _currentBgColor = Colors.Transparent;
    private readonly Windows.UI.Color _currentFgColor = Colors.Transparent;

    private async void Loaded(object sender, RoutedEventArgs e)
    {
        if (App.ConfigSet)
            await ConfigIsSet();
        else
            DisplaySettings();
    }

    public async Task ConfigIsSet()
    {
        if (App.LoadConfig() is { } exception)
        {
            DisplaySettings();
            await App.ExceptionHandler(exception);
        }
        else
        {
            _ = NavigateFrame.Navigate(typeof(IndexPage));
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        IconsHelper.LoadFilesIcons();

        foreach (NavigationViewItem menuItem in NavigationView.MenuItems)
            menuItem.IsEnabled = true;
        ((NavigationViewItem)NavigationView.FooterMenuItems[0]).IsEnabled = await App.ChangeFilesObserver();
    }

    private void DisplaySettings()
    {
        _ = NavigateFrame.Navigate(typeof(SettingsPage));
        NavigationView.SelectedItem = NavigationView.FooterMenuItems[1];
    }

    private void BackRequested(NavigationView navigationView, NavigationViewBackRequestedEventArgs e)
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
            SettingsPage => NavigationView.FooterMenuItems[1],
            _ => NavigationView.SelectedItem
        };
        NavigationView.IsBackEnabled = NavigateFrame.CanGoBack;
    }

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer.Tag is Type item && item != NavigateFrame.Content.GetType())
        {
            _ = NavigateFrame.Navigate(item);
            NavigationView.IsBackEnabled = true;
            GC.Collect();
        }
    }
    private void PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs e) => UpdateAppTitleMargin(sender);

    private void PaneOpening(NavigationView sender, object e) => UpdateAppTitleMargin(sender);

    private void DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs e)
    {
        var currentMargin = AppTitleBar.Margin;
        AppTitleBar.Margin = sender.DisplayMode is NavigationViewDisplayMode.Minimal
            ? new Thickness { Left = sender.CompactPaneLength * 2, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
            : new Thickness { Left = sender.CompactPaneLength, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };

        UpdateAppTitleMargin(sender);
    }

    private void UpdateAppTitleMargin(NavigationView sender)
    {
        const int smallLeftIndent = 4, largeLeftIndent = 24;

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            AppTitle.TranslationTransition = new Vector3Transition();

            AppTitle.Translation = (sender.DisplayMode is NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                     sender.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new System.Numerics.Vector3(smallLeftIndent, 0, 0)
                : new System.Numerics.Vector3(largeLeftIndent, 0, 0);
        }
        else
        {
            var currentMargin = AppTitle.Margin;

            AppTitle.Margin = (sender.DisplayMode is NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                     sender.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new Thickness { Left = smallLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
                : new Thickness { Left = largeLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };
        }
    }
}
