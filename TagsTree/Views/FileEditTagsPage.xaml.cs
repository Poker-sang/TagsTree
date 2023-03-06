using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Services;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class FileEditTagsPage : Page
{
    public FileEditTagsPage() => InitializeComponent();

    private readonly FileEditTagsViewModel _vm = new();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _vm.IsSaveEnabled = false;
        _vm.FileViewModel = e.Parameter.To<FileViewModel>();
    }

    #region 事件处理

    private async void AddTagTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var newTag = sender.GetTag<TagViewModel>();
        foreach (var tagExisted in _vm.VirtualTags)
            if (tagExisted.Name == newTag.Name)
            {
                SnackBarHelper.Show($"已拥有该标签「{newTag.Name}」", Severity.Error);
                return;
            }
            else if (newTag.HasChildTag(tagExisted))
            {
                SnackBarHelper.Show($"已拥有「{newTag.Name}」的下级标签「{tagExisted.Name}」或更多", Severity.Error);
                return;
            }
            else if (tagExisted.HasChildTag(newTag))
            {
                if (await ShowContentDialog.Warning($"「{newTag.Name}」将会覆盖上级标签「{tagExisted.Name}」，是否继续？"))
                {
                    _ = _vm.VirtualTags.Remove(tagExisted);
                    _vm.VirtualTags.Add(newTag);
                    _vm.IsSaveEnabled = true;
                }
                SnackBarHelper.Show($"「{newTag.Name}」覆盖上级标签「{tagExisted.Name}」");
                return;
            }

        _vm.VirtualTags.Add(newTag);
        _vm.IsSaveEnabled = true;
    }

    private void DeleteTagTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        _ = _vm.VirtualTags.Remove(sender.GetTag<TagViewModel>());
        _vm.IsSaveEnabled = true;
    }

    private void SaveTapped(object sender, TappedRoutedEventArgs e)
    {
        foreach (var tag in AppContext.Tags.TagsDictionaryValues)
            AppContext.Relations[tag.Id, _vm.FileViewModel.Id] = _vm.VirtualTags.Contains(tag);
        _vm.FileViewModel.TagsChanged();
        AppContext.SaveRelations();
        _vm.IsSaveEnabled = false;
        SnackBarHelper.Show("已保存更改");
    }

    #endregion
}
