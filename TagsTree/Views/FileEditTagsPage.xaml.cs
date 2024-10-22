using Microsoft.UI.Xaml;
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

    private FileEditTagsViewModel _vm = null!;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _vm = new(e.Parameter.To<FileViewModel>());
    }

    #region 事件处理

    private async void AddTagClicked(object sender, DoubleTappedRoutedEventArgs e)
    {
        var newTag = sender.To<FrameworkElement>().GetTag<TagViewModel>();
        foreach (var tagExisted in _vm.VirtualTags)
            if (tagExisted.Name == newTag.Name)
            {
                this.CreateTeachingTip().ShowAndHide($"已拥有该标签「{newTag.Name}」", TeachingTipSeverity.Error);
                return;
            }
            else if (newTag.HasChildTag(tagExisted))
            {
                this.CreateTeachingTip().ShowAndHide($"已拥有「{newTag.Name}」的下级标签「{tagExisted.Name}」或更多", TeachingTipSeverity.Error);
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
                this.CreateTeachingTip().ShowAndHide($"「{newTag.Name}」覆盖上级标签「{tagExisted.Name}」");
                return;
            }

        _vm.VirtualTags.Add(newTag);
        _vm.IsSaveEnabled = true;
    }

    private void DeleteTagClicked(object sender, DoubleTappedRoutedEventArgs e)
    {
        _ = _vm.VirtualTags.Remove(sender.To<FrameworkElement>().GetTag<TagViewModel>());
        _vm.IsSaveEnabled = true;
    }

    private void SaveClicked(object sender, RoutedEventArgs e)
    {
        foreach (var tag in AppContext.Tags.TagsDictionaryValues)
            AppContext.Relations[tag.Id, _vm.FileViewModel.Id] = _vm.VirtualTags.Contains(tag);
        _vm.FileViewModel.TagsChanged();
        AppContext.SaveRelations();
        _vm.IsSaveEnabled = false;
        this.CreateTeachingTip().ShowAndHide("已保存更改");
    }

    #endregion
}
