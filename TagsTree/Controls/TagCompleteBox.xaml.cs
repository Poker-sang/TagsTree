using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Controls;

/// <summary>
/// TagCompleteBox.xaml 的交互逻辑
/// </summary>
[INotifyPropertyChanged]
public partial class TagCompleteBox : UserControl
{
    public TagCompleteBox()
    {
        InitializeComponent();
        Switch();
    }

    private string _path = "";

    public string Path
    {
        get => _path;
        set
        {
            if (!Equals(_path, value))
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
                Tags.Clear();
                foreach (var s in Path.Split('\\'))
                    Tags.Add(s);
                Tags.Add("");
            }
            Switch();
        }
    }

    [ObservableProperty] private ObservableCollection<string> _tags = new();

    #region 事件处理

    private void PathComplement(object sender, RoutedEventArgs routedEventArgs) => Path = Path.GetTagViewModel()?.FullName ?? Path;
    private void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
    {
        Path = Regex.Replace(Path, $@"[{FileSystemHelper.GetInvalidPathChars}]+", "");
        sender.ItemsSource = sender.Text.TagSuggest('\\');
    }
    private void SuggestionChosen(AutoSuggestBox autoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs e)
    {
        Path = ((TagViewModel)e.SelectedItem).FullName;
        Switch();
    }

    private void OnLostFocus(object sender, RoutedEventArgs e) => Switch();

    #endregion

    private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args) => Switch(true);

    #region 操作

    private void Switch(bool force = false)
    {
        var isFocused = FocusState is not FocusState.Unfocused || Path is "" || force;
        AutoSuggestBox.IsHitTestVisible = isFocused;
        AutoSuggestBox.IsEnabled = isFocused;
        AutoSuggestBox.Opacity = isFocused ? 1 : 0;
        BreadcrumbBar.IsHitTestVisible = !isFocused;
        BreadcrumbBar.Opacity = isFocused ? 0 : 1;
    }

    #endregion
}