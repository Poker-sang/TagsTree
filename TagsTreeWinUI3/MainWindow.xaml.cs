﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using TagsTreeWinUI3.Views;
using Windows.UI;
using TagsTreeWinUI3.Services;

namespace TagsTreeWinUI3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            InitializeComponent();
            App.RootNavigationView = NavigationView;
            App.RootFrame = NavigateFrame;
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
            {
                if (App.LoadConfig() is { } exception)
                {
                    _ = NavigateFrame.Navigate(typeof(SettingsPage));
                    NavigationView.SelectedItem = NavigationView.FooterMenuItems[1];
                    NavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left; //不加就不会显示PaneTitle
                    OnPropertyChanged(nameof(PaneWidth));

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
                    _ = NavigateFrame.Navigate(typeof(TagEditFilesPage));
                else if (item == (string)((NavigationViewItem)sender.FooterMenuItems[0]).Content)
                    _ = NavigateFrame.Navigate(typeof(FilesObserverPage));
                else if (item == (string)((NavigationViewItem)sender.FooterMenuItems[1]).Content)
                    _ = NavigateFrame.Navigate(typeof(SettingsPage));
            NavigationView.IsBackEnabled = true;
            GC.Collect();
        }
    }
}