using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using WinUI3Utilities.Attributes;

namespace TagsTree.Controls;

[INotifyPropertyChanged]
[DependencyProperty<TagsTreeDictionary>("TagsSource", IsNullable = true, DefaultValue = "null")]
public partial class TagCompleteBox : UserControl
{
    public TagCompleteBox()
    {
        InitializeComponent();
        Switch(true);
    }

    private string _path = "";

    public string Path
    {
        get => _path;
        set
        {
            PathPrivate = value;
            Switch(false);
        }
    }

    private string PathPrivate
    {
        get => _path;
        set
        {
            if (!Equals(_path, value))
            {
                _path = value;
                OnPropertyChanged(nameof(PathPrivate));
                Tags.Clear();
                foreach (var s in PathPrivate.Split('\\'))
                    Tags.Add(s);
                //BreadcrumbBar中最后一个item无法点击，需要多加个空元素
                Tags.Add("");
            }
        }
    }

    [ObservableProperty] private ObservableCollection<string> _tags = new();

    #region 事件处理

    private void PathComplement(object sender, RoutedEventArgs routedEventArgs)
    {
        PathPrivate = PathPrivate.GetTagViewModel(TagsSource)?.FullName ?? PathPrivate;
        Switch(false);
    }

    private void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
    {
        PathPrivate = Regex.Replace(PathPrivate, $@"[{FileSystemHelper.GetInvalidPathChars}]+", "");
        sender.ItemsSource = sender.Text.TagSuggest('\\', TagsSource);
    }

    private void SuggestionChosen(AutoSuggestBox autoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs e)
    {
        PathPrivate = ((TagViewModel)e.SelectedItem).FullName;
        Switch(false);
    }

    private void TappedEnter(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => Switch(false);

    private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs e) => Switch(true);

    #endregion

    #region 操作

    /// <summary>
    /// 转换模式
    /// </summary>
    /// <param name="state"> <see langword="true"/>为输入模式（显示<see cref="AutoSuggestBox"/>），<see langword="false"/>为显示模式（显示<see cref="BreadcrumbBar"/>）</param>
    private void Switch(bool state)
    {
        var isFocused = Path is "" || state;
        AutoSuggestBox.IsHitTestVisible = isFocused;
        AutoSuggestBox.IsEnabled = isFocused;
        AutoSuggestBox.Opacity = isFocused ? 1 : 0;
        BreadcrumbBar.IsHitTestVisible = !isFocused;
        BreadcrumbBar.Opacity = isFocused ? 0 : 1;
    }

    #endregion
}
