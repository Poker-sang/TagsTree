using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Delegates;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using Windows.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3Utilities.Attributes;

namespace TagsTree.Controls;

[INotifyPropertyChanged]
[DependencyProperty<string>("Text", DefaultValue = @"""""")]
[DependencyProperty<TagsTreeDictionary>("TagsSource", IsNullable = true, DefaultValue = "null")]
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

    private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
    {
        if (Text is "")
        {
            ResultChanged.Invoke(App.IdFile.Values);
            return;
        }

        var temp = Text.Split(' ').Select(item => App.Tags.TagsDictionary.GetValueOrDefault(item) ?? new PathTagModel(item)).ToArray();
        ResultChanged.Invoke(App.Relations.GetFileModels(temp));
    }

    #endregion

    #region 操作

    public void ResetQuerySubmitted(TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> eventHandler)
    {
        AutoSuggestBox.QuerySubmitted -= QuerySubmitted;
        AutoSuggestBox.QuerySubmitted += eventHandler;
    }

    public void InvokeQuerySubmitted() => QuerySubmitted(AutoSuggestBox, new AutoSuggestBoxQuerySubmittedEventArgs());

    #endregion
}
