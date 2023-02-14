using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using TagsTree.Interfaces;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class SelectTagToEditPage : Page, ITypeGetter
{
    public SelectTagToEditPage() => InitializeComponent();
    public static Type TypeGetter => typeof(SelectTagToEditPage);

    private readonly SelectTagToEditPageViewModel _vm = new();

    #region 事件处理

    private void TagsOnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => _vm.Path = e.InvokedItem.To<TagViewModel?>()?.FullName ?? _vm.Path;

    private async void ConfirmTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_vm.Path.GetTagViewModel() is not { } pathTagModel)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不存在！");
            return;
        }

        if (pathTagModel == AppContext.Tags.TagsDictionaryRoot)
        {
            await ShowMessageDialog.Information(true, "「标签路径」不能为空！");
            return;
        }

        NavigationHelper.GotoPage<TagEditFilesPage>(pathTagModel);
    }

    #endregion
}
