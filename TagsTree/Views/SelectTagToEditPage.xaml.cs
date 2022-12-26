using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using TagsTree.Interfaces;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

public partial class SelectTagToEditPage : Page, ITypeGetter
{
    public SelectTagToEditPage() => InitializeComponent();
    public static Type TypeGetter => typeof(SelectTagToEditPage);

    /// <summary>
    /// 不为<see langword="static"/>方便绑定
    /// </summary>
    private ObservableCollection<TagViewModel> Vm => App.Tags.TagsTree.SubTags;

    #region 事件处理

    private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = (e.InvokedItem as TagViewModel)?.FullName ?? TbPath.Path;

    private async void ConfirmClick(object sender, RoutedEventArgs e)
    {
        if (TbPath.Path.GetTagViewModel() is not { } pathTagModel)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不存在！");
            return;
        }

        if (pathTagModel == App.Tags.TagsDictionaryRoot)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不能为空！");
            return;
        }

        _ = App.RootFrame.Navigate(typeof(TagEditFilesPage), pathTagModel);
    }

    #endregion
}
