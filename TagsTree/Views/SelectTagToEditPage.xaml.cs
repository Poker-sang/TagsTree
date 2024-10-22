using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Interfaces;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class SelectTagToEditPage : Page, ITypeGetter
{
    public SelectTagToEditPage() => InitializeComponent();

    public static Type TypeGetter => typeof(SelectTagToEditPage);

    private readonly SelectTagToEditPageViewModel _vm = new();

    #region 事件处理

    private void TagsOnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => _vm.Path = e.InvokedItem.To<TagViewModel?>()?.FullName ?? _vm.Path;

    private async void ConfirmClicked(object sender, RoutedEventArgs e)
    {
        if (_vm.Path.GetTagViewModel() is not { } pathTagModel)
        {
            await ShowContentDialog.Information(true, "「标签路径」不存在！");
            return;
        }

        if (pathTagModel == AppContext.Tags.TagsDictionaryRoot)
        {
            await ShowContentDialog.Information(true, "「标签路径」不能为空！");
            return;
        }

        App.MainWindow.GotoPage<TagEditFilesPage>(pathTagModel);
    }

    #endregion
}
