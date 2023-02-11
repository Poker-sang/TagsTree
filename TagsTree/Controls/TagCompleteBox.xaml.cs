using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using TagsTree.ViewModels.Controls;
using WinUI3Utilities;
using WinUI3Utilities.Attributes;

namespace TagsTree.Controls;

[INotifyPropertyChanged]
[DependencyProperty<TagsTreeDictionary>("TagsSource", IsNullable = true, DefaultValue = "null")]
public partial class TagCompleteBox : UserControl
{
    private readonly TagCompleteBoxViewModel _vm;

    public TagCompleteBox()
    {
        _vm = new(this);
        InitializeComponent();
        _vm.State = true;
    }

    public string Path
    {
        get => _vm.Path;
        set
        {
            _vm.Path = value;
            _vm.State = false;
        }
    }

    #region 事件处理

    private void PathComplement(object sender, RoutedEventArgs e) => _vm.Path = _vm.Path.GetTagViewModel(TagsSource)?.FullName ?? _vm.Path;

    private void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => _vm.Path = Regex.Replace(_vm.Path, $@"[{FileSystemHelper.GetInvalidPathChars}]+", "");

    private void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e) => _vm.Path = e.SelectedItem.To<TagViewModel>().FullName;

    private void TappedEnter(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e) => _vm.State = false;

    private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs e) => _vm.State = true;

    private void AutoSuggestBoxIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var asb = sender.To<AutoSuggestBox>();
        if (asb.IsEnabled)
            _ = asb.Focus(FocusState.Programmatic);
    }

    #endregion
}
