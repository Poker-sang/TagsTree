using System;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Interfaces;

namespace TagsTree.Views;

public sealed partial class IndexPage : Page, ITypeGetter
{
    public IndexPage()
    {
        InitializeComponent();
        TagSearchBox.ResetQuerySubmitted(QuerySubmitted);
    }

    public static Type TypeGetter => typeof(IndexPage);

    #region 事件处理

    private static void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => App.MainWindow.GotoPage<TagSearchFilesPage>(e.QueryText);

    #endregion
}
