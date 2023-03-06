using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class TagSearchFilesPage : Page
{
    public TagSearchFilesPage()
    {
        _vm = new();
        _current = this;
        InitializeComponent();
        TbSearch.InvokeQuerySubmitted();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) => TbSearch.Text = e.Parameter.To<string>();

    private readonly TagSearchFilesViewModel _vm;

    #region 事件处理

    private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.ResultCallBack = newResult.Select(fileModel => new FileViewModel(fileModel)).ToObservableCollection();

    private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => sender.Text = Regex.Replace(sender.Text, $@"[{FileSystemHelper.GetInvalidNameChars} ]+", "");

    private void QuerySubmitted(AutoSuggestBox autoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs e) => _vm.FileViewModels = autoSuggestBox.Text is "" ? _vm.ResultCallBack : RelationsDataTable.FuzzySearchName(autoSuggestBox.Text, _vm.ResultCallBack);

    private void ContextOpenTapped(object sender, TappedRoutedEventArgs e) => sender.GetDataContext<FileViewModel>().Open();

    private void ContextOpenExplorerTapped(object sender, TappedRoutedEventArgs e) => sender.GetDataContext<FileViewModel>().OpenDirectory();

    private async void ContextRemoveTapped(object sender, TappedRoutedEventArgs e)
    {
        var fileViewModel = sender.GetDataContext<FileViewModel>();
        // 打开确认框会关闭菜单，导致DataContext变为null，所以提前记录
        if (!await ShowContentDialog.Warning("是否从软件移除该文件？"))
            return;
        fileViewModel.RemoveAndSave();
        _ = _vm.FileViewModels.Remove(fileViewModel);
    }

    private void ContextPropertiesTapped(object sender, TappedRoutedEventArgs e) => NavigationHelper.GotoPage<FilePropertiesPage>(sender.GetDataContext<FileViewModel>());

    private void ContextPropertiesDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender.To<DataGrid>().SelectedItem is FileViewModel fileViewModel)
            NavigationHelper.GotoPage<FilePropertiesPage>(fileViewModel);
    }

    #endregion

    #region 操作

    private static TagSearchFilesPage _current = null!;

    public static void FileRemoved(FileViewModel removedItem) => _ = _current._vm.FileViewModels.Remove(removedItem);

    #endregion
}
