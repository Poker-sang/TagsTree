using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

public partial class TagSearchFilesPage : Page
{
    public TagSearchFilesPage()
    {
        _vm = new TagSearchFilesViewModel();
        _current = this;
        InitializeComponent();
        TbSearch.InvokeQuerySubmitted();
    }
    public static Type TypeGetter => typeof(TagSearchFilesPage);
    protected override void OnNavigatedTo(NavigationEventArgs e) => TbSearch.Text = (string)e.Parameter;

    private readonly TagSearchFilesViewModel _vm;

    #region 事件处理

    private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.ResultCallBack = newResult.Select(fileModel => new FileViewModel(fileModel)).ToObservableCollection();

    private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => sender.Text = Regex.Replace(sender.Text, $@"[{FileSystemHelper.GetInvalidNameChars} ]+", "");

    private void QuerySubmitted(AutoSuggestBox autoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs e) => _vm.FileViewModels = autoSuggestBox.Text is "" ? _vm.ResultCallBack : RelationsDataTable.FuzzySearchName(autoSuggestBox.Text, _vm.ResultCallBack);

    private void OpenCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).DataContext).Open();
    private void OpenExplorerCmClick(object sender, RoutedEventArgs e) => ((FileViewModel)((MenuFlyoutItem)sender).DataContext).OpenDirectory();
    private async void RemoveCmClick(object sender, RoutedEventArgs e)
    {
        if (!await ShowMessageDialog.Warning("是否从软件移除该文件？"))
            return;
        ((FileViewModel)((FrameworkElement)sender).DataContext).RemoveAndSave();
        _ = _vm.FileViewModels.Remove((FileViewModel)((FrameworkElement)sender).DataContext);
    }
    private void PropertiesCmClick(object sender, RoutedEventArgs e) => App.RootFrame.Navigate(typeof(FilePropertiesPage), (FileViewModel)((MenuFlyoutItem)sender).DataContext);

    private void PropertiesCmDoubleClick(object sender, RoutedEventArgs e)
    {
        if ((FileViewModel)((DataGrid)sender).SelectedItem is { } fileViewModel)
            _ = App.RootFrame.Navigate(typeof(FilePropertiesPage), fileViewModel);
    }

    #endregion

    #region 操作

    private static TagSearchFilesPage _current = null!;
    public static void FileRemoved(FileViewModel removedItem) => _ = _current._vm.FileViewModels.Remove(removedItem);

    #endregion
}