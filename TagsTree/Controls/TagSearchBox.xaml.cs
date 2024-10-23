using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Delegates;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using Windows.Foundation;
using WinUI3Utilities.Attributes;

namespace TagsTree.Controls;

[INotifyPropertyChanged]
[DependencyProperty<string>("Text", "\"\"")]
[DependencyProperty<TagsTreeDictionary>("TagsSource", DependencyPropertyDefaultValue.Default, IsNullable = true)]
public partial class TagSearchBox : UserControl
{
    public TagSearchBox() => InitializeComponent();

    public event ResultChangedEventHandler ResultChanged = null!;

    #region 事件处理

    private void SuggestionChosen(AutoSuggestBox autoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs e)
    {
        // TODO: SuggestionChosen
        //    var index = Text.LastIndexOf(' ') + 1;
        //    if (index is 0)
        //        Text = e.SelectedItem.ToString();
        //    else Text = Text[..index] + e.SelectedItem;
    }

    [GeneratedRegex("  +")]
    private static partial Regex MyRegex();

    private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
    {
        Text = Regex.Replace(Text, $@"[{FileSystemHelper.GetInvalidPathChars}]+", "");
        Text = MyRegex().Replace(Text, " ").TrimStart();
        sender.ItemsSource = Text.TagSuggest(' ', TagsSource);
    }

    private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => SubmitQuery(e.QueryText);

    public void SubmitQuery(string queryText)
    {
        if (queryText is "")
        {
            ResultChanged.Invoke(AppContext.IdFile.Values);
            return;
        }

        var temp = queryText.Split(' ').Select(item => 
            AppContext.Tags.TagsDictionary.GetValueOrDefault(item) ?? new PathTagModel(item)).ToArray();
        ResultChanged.Invoke(RelationsDataTable.GetFileModels(temp));
    }

    #endregion

    #region 操作

    public void ResetQuerySubmitted(TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> eventHandler)
    {
        AutoSuggestBox.QuerySubmitted -= QuerySubmitted;
        AutoSuggestBox.QuerySubmitted += eventHandler;
    }

    #endregion
}
