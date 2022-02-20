using Microsoft.UI.Xaml.Controls;
using TagsTree.Interfaces;

namespace TagsTree.Views;

/// <summary>
/// IndexPage.xaml 的交互逻辑
/// </summary>
public sealed partial class IndexPage : Page, ITypeName
{
    public IndexPage()
    {
        InitializeComponent();
        TagSearchBox.ResetQuerySubmitted(QuerySubmitted);
    }

    public static string TypeName => nameof(IndexPage);

    #region 事件处理

    private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => _ = App.RootFrame.Navigate(typeof(TagSearchFilesPage), TagSearchBox.Text);

    #endregion

}