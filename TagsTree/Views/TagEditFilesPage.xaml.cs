using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public sealed partial class TagEditFilesPage : Page
{
    public TagEditFilesPage() => InitializeComponent();

    private readonly TagEditFilesViewModel _vm = new();

    #region 事件处理

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _vm.TagViewModel = e.Parameter.To<TagViewModel>();
        _vm.FileViewModels = AppContext.Relations.GetFileModels().Select(fileModel => new FileViewModel(fileModel, _vm.TagViewModel)).ToObservableCollection();
    }

    private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.FileViewModels = newResult.Select(fileModel => new FileViewModel(fileModel, _vm.TagViewModel.Parent)).ToObservableCollection();


    private void Selected(object sender, SelectionChangedEventArgs e)
    {
        var dg = sender.To<DataGrid>();
        if (dg.SelectedItem.To<FileViewModel>() is not { } item)
            return;
        item.SelectedFlip();
        dg.SelectedIndex = -1;
    }

    private void SaveTapped(object sender, TappedRoutedEventArgs e)
    {
        foreach (var fileViewModel in _vm.FileViewModels)
            if (fileViewModel.Selected != fileViewModel.SelectedOriginal)
            {
                switch (fileViewModel.SelectedOriginal)
                {
                    case true:
                        AppContext.Relations[_vm.TagViewModel.Id, fileViewModel.Id] = false;
                        break;
                    // 如果原本有上级标签，则覆盖相应上级标签
                    case false:
                        AppContext.Relations[_vm.TagViewModel.Id, fileViewModel.Id] = true;
                        foreach (var tagViewModel in fileViewModel.Tags.GetTagViewModels())
                            if (tagViewModel.HasChildTag(_vm.TagViewModel))
                            {
                                AppContext.Relations[tagViewModel.Id, fileViewModel.Id] = false;
                                break;
                            }

                        break;
                    // 如果原本是null，则删除fileViewModel拥有的相应子标签
                    case null:
                        foreach (var tag in fileViewModel.GetAncestorTags(_vm.TagViewModel))
                            AppContext.Relations[tag.Id, fileViewModel.Id] = false;
                        break;
                }

                fileViewModel.TagsChanged();
            }

        AppContext.SaveRelations();
        SnackBarHelper.Show("已保存更改");
        CurrentContext.Frame.GoBack(new SlideNavigationTransitionInfo());
    }

    #endregion
}
