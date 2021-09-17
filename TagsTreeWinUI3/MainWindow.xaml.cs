using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;
using TagsTreeWinUI3.Views;
using Windows.UI;

namespace TagsTreeWinUI3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetTitleBar(TitleBar);
        }

        public bool SetFilesObserverEnable { set => ((NavigationViewItem)NavigationView.MenuItems[4]).IsEnabled = value; }

        private void Loaded(object sender, RoutedEventArgs e) => _ = NavigateFrame.Navigate(typeof(SettingsPage));

        public async void ConfigModeUnlock()
        {
            foreach (NavigationViewItem menuItem in NavigationView.MenuItems)
                menuItem.IsEnabled = true;
            await Task.Delay(500);
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
            _ = NavigateFrame.Navigate(typeof(IndexPage));
        }
        private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
        {
            NavigateFrame.GoBack();
            sender.SelectedItem = NavigateFrame.Content switch
            {
                IndexPage => sender.MenuItems[0],
                TagsManagerPage => sender.MenuItems[1],
                FileImporterPage => sender.MenuItems[2],
                TagEditFilesPage => sender.MenuItems[3],
                FilesObserverPage => sender.MenuItems[4],
                SettingsPage => sender.FooterMenuItems[0],
                _ => sender.SelectedItem
            };
            NavigationView.IsBackEnabled = NavigateFrame.CanGoBack;
        }

        /// <summary>
        /// 不为static方便绑定
        /// </summary>
        private Brush SystemColor => new SolidColorBrush(Application.Current.RequestedTheme is ApplicationTheme.Light ? Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF) : Color.FromArgb(0x65, 0x00, 0x00, 0x00));

        private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
        {
            if ((string)((NavigationViewItem)sender.SelectedItem).Tag == NavigateFrame.Content.GetType().Name) return;
            if ((string)e.InvokedItem == (string)((NavigationViewItem)sender.MenuItems[0]).Content)
                _ = NavigateFrame.Navigate(typeof(IndexPage));
            else if ((string)e.InvokedItem == (string)((NavigationViewItem)sender.MenuItems[1]).Content)
                _ = NavigateFrame.Navigate(typeof(TagsManagerPage));
            else if ((string)e.InvokedItem == (string)((NavigationViewItem)sender.MenuItems[2]).Content)
                _ = NavigateFrame.Navigate(typeof(FileImporterPage));
            else if ((string)e.InvokedItem == (string)((NavigationViewItem)sender.MenuItems[3]).Content)
                _ = NavigateFrame.Navigate(typeof(TagEditFilesPage));
            else if ((string)e.InvokedItem == (string)((NavigationViewItem)sender.MenuItems[4]).Content)
                _ = NavigateFrame.Navigate(typeof(FilesObserverPage));
            else if ((string)e.InvokedItem == (string)((NavigationViewItem)sender.FooterMenuItems[0]).Content)
                _ = NavigateFrame.Navigate(typeof(SettingsPage));
            NavigationView.IsBackEnabled = true;
            GC.Collect();
        }
    }
}