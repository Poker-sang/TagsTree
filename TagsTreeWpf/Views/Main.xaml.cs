using JetBrains.Annotations;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TagsTreeWpf.Services;
using static TagsTreeWpf.Properties.Settings;
using Frame = System.Windows.Controls.Frame;

namespace TagsTreeWpf.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window, INotifyPropertyChanged
    {
        public Main()
        {
            App.Win = this;
            InitializeComponent();
            _ = Frame.Navigate(new IndexPage());
            if (!CheckConfig()) Close();
        }
        #region 操作

        private static bool CheckConfig()
        {
            ThemeManager.Current.ApplicationTheme = Default.Theme ? ApplicationTheme.Dark : ApplicationTheme.Light;
            while (true)
            {
                if (Default.IsSet) //如果之前有储存过用户配置，则判断是否符合
                    switch (App.LoadConfig())
                    {
                        case null: return false;
                        case true: return true;
                    }
                else if (new NewConfig().ShowDialog() == false)
                    return false;
            }
        }

        #endregion
        private double PaneWidth => Math.Max(NavigationView.ActualWidth, NavigationView.CompactModeThresholdWidth) / 4;

        private void ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
        {
            if (e.InvokedItem is string item)
                if (Equals(((NavigationViewItem)sender.SelectedItem).Content, Frame.Content.GetType().Name))
                    return;
                else if (Equals(item, ((NavigationViewItem)sender.FooterMenuItems[0]!).Content))
                {
                    if (MessageBoxX.Warning("更改设置后需要重启软件，请确保已保存") && new NewConfig().ShowDialog() == true)
                        Close();
                }
                else _ = Frame.Navigate(0 switch
                {
                    0 when Equals(item, ((NavigationViewItem)sender.MenuItems[0]!).Content) => new IndexPage(),
                    0 when Equals(item, ((NavigationViewItem)sender.MenuItems[1]!).Content) => new TagsManagerPage(),
                    0 when Equals(item, ((NavigationViewItem)sender.MenuItems[2]!).Content) => new FileImporterPage(),
                    0 when Equals(item, ((NavigationViewItem)sender.MenuItems[3]!).Content) => new TagEditFilesPage(),
                    _ => throw new ArgumentOutOfRangeException()
                });
            sender.IsBackEnabled = true;
            GC.Collect();
        }

        private void BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
        {
            Frame.GoBack();
            sender.SelectedItem = Frame.Content switch
            {
                IndexPage => sender.MenuItems[0],
                TagsManagerPage => sender.MenuItems[1],
                FileImporterPage => sender.MenuItems[2],
                TagEditFilesPage => sender.MenuItems[3],
                NewConfig => sender.FooterMenuItems[0],
                _ => sender.SelectedItem
            };
            sender.IsBackEnabled = Frame.CanGoBack;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            NavigationView.PaneDisplayMode = NavigationView.ActualWidth < NavigationView.CompactModeThresholdWidth ? NavigationViewPaneDisplayMode.LeftCompact : NavigationViewPaneDisplayMode.Left;
            OnPropertyChanged(nameof(PaneWidth));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}