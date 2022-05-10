using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using TagsTree.Services;
using TagsTree.Views;

namespace TagsTree;

[INotifyPropertyChanged]
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        ExtendsContentIntoTitleBar = true;
        InitializeComponent();
        App.RootNavigationView = NavigationView;
        App.RootFrame = NavigateFrame;
        //TODO 标题栏
        SetTitleBar(TitleBar);
    }

    private double PaneWidth => Math.Max(NavigationView.ActualWidth, NavigationView.CompactModeThresholdWidth) / 4;
    private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
    {
        NavigationView.PaneDisplayMode = NavigationView.ActualWidth < NavigationView.CompactModeThresholdWidth ? NavigationViewPaneDisplayMode.LeftCompact : NavigationViewPaneDisplayMode.Left;
        OnPropertyChanged(nameof(PaneWidth));
    }

    private async void Loaded(object sender, RoutedEventArgs e)
    {
        if (App.ConfigSet)
            await ConfigIsSet();
        else DisplaySettings();
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
            NavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left; //不加就不会显示PaneTitle
            OnPropertyChanged(nameof(PaneWidth));
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
        NavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left; //不加就不会显示PaneTitle
        OnPropertyChanged(nameof(PaneWidth));
    }

    private void BackRequested(object sender, RoutedEventArgs e)
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
        BBackRequest.IsEnabled = NavigateFrame.CanGoBack;
    }

    private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer.Tag is Type item && item != NavigateFrame.Content.GetType())
        {
            _ = NavigateFrame.Navigate(item);
            BBackRequest.IsEnabled = true;
            GC.Collect();
        }
    }
}