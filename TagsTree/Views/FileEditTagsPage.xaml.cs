using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

public partial class FileEditTagsPage : Page
{
    public FileEditTagsPage()
    {
        _vm = new();
        InitializeComponent();
    }

    private readonly FileEditTagsViewModel _vm;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        BSave.IsEnabled = false;
        _vm.Load((FileViewModel)e.Parameter);
    }

    #region 事件处理

    private void AddTag(object sender, DoubleTappedRoutedEventArgs e)
    {
        var newTag = sender.GetTag<TagViewModel>();
        foreach (var tagExisted in _vm.VirtualTags)
            if (tagExisted.Name == newTag.Name)
            {
                InfoBar.Severity = InfoBarSeverity.Error;
                InfoBar.Title = "错误";
                InfoBar.Message = $"已拥有该标签「{newTag.Name}」";
                InfoBar.IsOpen = true;
                return;
            }
            else if (newTag.HasChildTag(tagExisted))
            {
                InfoBar.Severity = InfoBarSeverity.Error;
                InfoBar.Title = "错误";
                InfoBar.Message = $"已拥有「{newTag.Name}」的下级标签「{tagExisted.Name}」或更多";
                InfoBar.IsOpen = true;
                return;
            }
            else if (tagExisted.HasChildTag(newTag))
            {
                InfoBar.Severity = InfoBarSeverity.Warning;
                InfoBar.Title = "警告";
                InfoBar.Message = $"「{newTag.Name}」将会覆盖上级标签「{tagExisted.Name}」，是否继续？";
                InfoBar.ActionButton = new Button { Content = "确认" };
                InfoBar.ActionButton.Click += (_, _) =>
                {
                    _ = _vm.VirtualTags.Remove(tagExisted);
                    _vm.VirtualTags.Add(newTag);
                    BSave.IsEnabled = true;
                    InfoBar.ActionButton = null;
                    InfoBar.IsOpen = false;
                };
                InfoBar.IsOpen = true;
                return;
            }

        _vm.VirtualTags.Add(newTag);
        BSave.IsEnabled = true;
    }

    private void DeleteTag(object sender, RoutedEventArgs e)
    {
        _ = _vm.VirtualTags.Remove(sender.GetTag<TagViewModel>());
        BSave.IsEnabled = true;
    }

    private void SaveBClick(object sender, RoutedEventArgs e)
    {
        foreach (var tag in App.Tags.TagsDictionaryValues)
            App.Relations[tag.Id, _vm.FileViewModel.Id] = _vm.VirtualTags.Contains(tag);
        _vm.FileViewModel.TagsUpdated();
        App.SaveRelations();
        BSave.IsEnabled = false;
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.Title = "成功";
        InfoBar.Message = "已保存更改";
        InfoBar.IsOpen = true;
    }
    #endregion

}
