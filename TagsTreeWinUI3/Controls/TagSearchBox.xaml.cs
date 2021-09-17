using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Delegates;
using TagsTreeWinUI3.Models;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;
using Windows.Foundation;

namespace TagsTreeWinUI3.Controls
{
    /// <summary>
    /// TagSearchBox.xaml 的交互逻辑
    /// </summary>
    public partial class TagSearchBox : UserControl
    {

        public TagSearchBox() => InitializeComponent();

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> BeforeQuerySubmitted
        {
            add
            {
                TokenizingTextBox.QuerySubmitted -= QuerySubmitted;
                TokenizingTextBox.QuerySubmitted += value;
                TokenizingTextBox.QuerySubmitted += QuerySubmitted;
            }
            remove
            {
                TokenizingTextBox.QuerySubmitted -= QuerySubmitted;
                TokenizingTextBox.QuerySubmitted -= value;
                TokenizingTextBox.QuerySubmitted += QuerySubmitted;
            }
        }

        public event ResultChangedEventHandler ResultChanged = null!;

        private void SuggestionChosen(AutoSuggestBox autoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            var index = autoSuggestBox.Text.LastIndexOf(' ') + 1;
            if (index is 0)
                autoSuggestBox.Text = e.SelectedItem.ToString();
            else autoSuggestBox.Text = autoSuggestBox.Text[..index] + e.SelectedItem;
        }
        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (TokenizingTextBox.Text is "" && TokenizingTextBox.Items.Count is 1 && TokenizingTextBox.Items.First().ToString() is { } and not { Length: 0 })
            {
                ResultChanged.Invoke(App.IdFile.Values.Select(fileModel => new FileViewModel(fileModel)));
                return;
            }
            sender.Text = Regex.Replace(sender.Text, $@"[{FileX.GetInvalidPathChars}]+", "");
            sender.Text = Regex.Replace(sender.Text, @"  +", " ").TrimStart();
            sender.ItemsSource = sender.Text.TagSuggest(' ');
        }
        private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            var temp = new List<PathTagModel>();
            foreach (var item in TokenizingTextBox.Items)
                if (item.ToString() is { } name and not { Length: 0 })
                    if (App.Tags.TagsDictionary.GetValueOrDefault(name) is { } tagModel)
                        temp.Add(tagModel);
                    else temp.Add(new PathTagModel(name));
            ResultChanged.Invoke(App.Relations.GetFileModels(temp));
        }
    }
}
