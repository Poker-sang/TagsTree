using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TagsTree.Services;
using TagsTree.Views;
using Windows.UI;

namespace TagsTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.RootNavigationView = NavigationView;
            App.RootFrame = NavigateFrame;
            //SetTitleBar(TitleBar);
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
            ((NavigationViewItem)NavigationView.FooterMenuItems[0]).IsEnabled = await App.ChangeFilesObserver(); //就是App.AppConfigurations.FilesObserverEnabled;
        }

        private void DisplaySettings()
        {
            _ = NavigateFrame.Navigate(typeof(SettingsPage));
            NavigationView.SelectedItem = NavigationView.FooterMenuItems[1];
            NavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left; //不加就不会显示PaneTitle
            OnPropertyChanged(nameof(PaneWidth));
        }

        private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
        {
            NavigateFrame.GoBack();
            sender.SelectedItem = NavigateFrame.Content switch
            {
                IndexPage => sender.MenuItems[0],
                FilePropertiesPage => sender.MenuItems[0],
                FileEditTagsPage => sender.MenuItems[0],
                TagsManagerPage => sender.MenuItems[1],
                FileImporterPage => sender.MenuItems[2],
                SelectTagToEditPage => sender.MenuItems[3],
                TagEditFilesPage => sender.MenuItems[3],
                FilesObserverPage => sender.FooterMenuItems[0],
                SettingsPage => sender.FooterMenuItems[1],
                _ => sender.SelectedItem
            };
            NavigationView.IsBackEnabled = NavigateFrame.CanGoBack;
        }

        /// <summary>
        /// 不为static方便绑定
        /// </summary>
        private readonly Brush _systemColor = new SolidColorBrush(Application.Current.RequestedTheme is ApplicationTheme.Light ? Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF) : Color.FromArgb(0x65, 0x00, 0x00, 0x00));

        private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
        {
            if (e.InvokedItem is string item)
                if ((string)((NavigationViewItem)sender.SelectedItem).Tag == NavigateFrame.Content.GetType().Name)
                    return;
                else if (item == (string)((NavigationViewItem)sender.MenuItems[0]).Content)
                    _ = NavigateFrame.Navigate(typeof(IndexPage));
                else if (item == (string)((NavigationViewItem)sender.MenuItems[1]).Content)
                    _ = NavigateFrame.Navigate(typeof(TagsManagerPage));
                else if (item == (string)((NavigationViewItem)sender.MenuItems[2]).Content)
                    _ = NavigateFrame.Navigate(typeof(FileImporterPage));
                else if (item == (string)((NavigationViewItem)sender.MenuItems[3]).Content)
                    _ = NavigateFrame.Navigate(typeof(SelectTagToEditPage));
                else if (item == (string)((NavigationViewItem)sender.FooterMenuItems[0]).Content)
                    _ = NavigateFrame.Navigate(typeof(FilesObserverPage));
                else if (item == (string)((NavigationViewItem)sender.FooterMenuItems[1]).Content)
                    _ = NavigateFrame.Navigate(typeof(SettingsPage));
            NavigationView.IsBackEnabled = true;
            GC.Collect();
        }
    }
}