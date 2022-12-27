using Microsoft.UI.Xaml.Controls;
using System;
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

    private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => App.GotoPage<TagSearchFilesPage>(TagSearchBox.Text);

    #endregion
}
