using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

/// <summary>
/// SelectTagToEditPage.xaml 的交互逻辑
/// </summary>
public partial class SelectTagToEditPage : Page
{
    public SelectTagToEditPage() => InitializeComponent();

    /// <summary>
    /// 不为static方便绑定
    /// </summary>
    private ObservableCollection<TagViewModel> Vm => App.Tags.TagsTree.SubTags;


    #region 事件处理

    private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = ((TagViewModel?)e.InvokedItem)?.FullName ?? TbPath.Path;

    private async void ConfirmBClick(object sender, RoutedEventArgs e)
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
        App.RootFrame.Navigate(typeof(TagEditFilesPage), pathTagModel);
    }

    #endregion
}