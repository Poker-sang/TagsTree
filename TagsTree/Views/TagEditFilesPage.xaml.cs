using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public sealed partial class TagEditFilesPage : Page
{
    public TagEditFilesPage()
    {
        _vm = new();
        InitializeComponent();
    }

    private readonly TagEditFilesViewModel _vm;

    #region 事件处理

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _vm.TagViewModel = (TagViewModel)e.Parameter;
        _vm.FileViewModels = App.Relations.GetFileModels().Select(fileModel => new FileViewModel(fileModel, _vm.TagViewModel)).ToObservableCollection();
    }

    private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.FileViewModels = newResult.Select(fileModel => new FileViewModel(fileModel, _vm.TagViewModel.Parent)).ToObservableCollection();

    private void Selected(object sender, SelectionChangedEventArgs e)
    {
        if ((FileViewModel)((DataGrid)sender).SelectedItem is not { } item)
            return;
        item.SelectedFlip();
        ((DataGrid)sender).SelectedIndex = -1;
    }
    private async void SaveClick(object sender, RoutedEventArgs e)
    {

        foreach (var fileViewModel in _vm.FileViewModels)
            if (fileViewModel.Selected != fileViewModel.SelectedOriginal)
            {
                switch (fileViewModel.SelectedOriginal)
                {
                    case true:
                        App.Relations[_vm.TagViewModel.Id, fileViewModel.Id] = false;
                        break;
                    // 如果原本有上级标签，则覆盖相应上级标签
                    case false:
                        App.Relations[_vm.TagViewModel.Id, fileViewModel.Id] = true;
                        foreach (var tagViewModel in fileViewModel.Tags.GetTagViewModels())
                            if (tagViewModel.HasChildTag(_vm.TagViewModel))
                            {
                                App.Relations[tagViewModel.Id, fileViewModel.Id] = false;
                                break;
                            }

                        break;
                    // 如果原本是null，则删除fileViewModel拥有的相应子标签
                    case null:
                        foreach (var tag in fileViewModel.GetAncestorTags(_vm.TagViewModel))
                            App.Relations[tag.Id, fileViewModel.Id] = false;
                        break;
                }

                fileViewModel.TagsUpdated();
            }

        App.SaveRelations();
        await ShowMessageDialog.Information(false, "已保存更改");
        App.RootFrame.GoBack(new SlideNavigationTransitionInfo());
    }

    #endregion
}
