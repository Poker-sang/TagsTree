using Microsoft.UI.Xaml.Controls;
using System;

namespace TagsTree.Views;

public sealed partial class IndexPage : Page
{
    public IndexPage()
    {
        InitializeComponent();
        TagSearchBox.ResetQuerySubmitted(QuerySubmitted);
    }

    public static Type TypeGetter => typeof(IndexPage);

    #region 事件处理

    private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => _ = App.RootFrame.Navigate(typeof(TagSearchFilesPage), TagSearchBox.Text);

    #endregion

}