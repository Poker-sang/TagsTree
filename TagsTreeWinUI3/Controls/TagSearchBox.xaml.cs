using System;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TagsTree.Delegates;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using Windows.Foundation;

namespace TagsTree.Controls
{
    /// <summary>
    /// TagSearchBox.xaml 的交互逻辑
    /// </summary>
    public partial class TagSearchBox : UserControl
    {

        public TagSearchBox() => InitializeComponent();
        
        public string Text
        {
            get => AutoSuggestBox.Text;
            set => AutoSuggestBox.Text = value;
        }

        public event ResultChangedEventHandler ResultChanged = null!;

        #region 事件处理

        private void SuggestionChosen(AutoSuggestBox autoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            //    var index = autoSuggestBox.Text.LastIndexOf(' ') + 1;
            //    if (index is 0)
            //        autoSuggestBox.Text = e.SelectedItem.ToString();
            //    else autoSuggestBox.Text = autoSuggestBox.Text[..index] + e.SelectedItem;
        }
        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            sender.Text = Regex.Replace(sender.Text, $@"[{FileSystemHelper.GetInvalidPathChars}]+", "");
            sender.Text = Regex.Replace(sender.Text, @"  +", " ").TrimStart();
            sender.ItemsSource = sender.Text.TagSuggest(' ');
        }
        private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            if (sender.Text is "")
            {
                ResultChanged.Invoke(App.IdFile.Values);
                return;
            }
            var temp = new List<PathTagModel>();
            foreach (var item in sender.Text.Split(' '))
                if (App.Tags.TagsDictionary.GetValueOrDefault(item) is { } tagModel)
                    temp.Add(tagModel);
                else temp.Add(new PathTagModel(item));
            ResultChanged.Invoke(App.Relations.GetFileModels(temp));
        }

        #endregion

        #region 操作

        public void ResetQuerySubmitted(TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> eventHandler)
        {
            AutoSuggestBox.QuerySubmitted -= QuerySubmitted;
            AutoSuggestBox.QuerySubmitted += eventHandler;
        }

        public void InvokeQuerySubmitted() => QuerySubmitted(AutoSuggestBox, null!);

        #endregion
    }
}
