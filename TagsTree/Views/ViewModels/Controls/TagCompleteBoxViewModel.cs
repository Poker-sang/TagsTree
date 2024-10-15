using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using TagsTree.Controls;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Views.ViewModels.Controls;

public partial class TagCompleteBoxViewModel(TagCompleteBox tcb) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Tags))]
    [NotifyPropertyChangedFor(nameof(SuggestionTags))]
    private string _path = "";

    /// <summary>
    /// BreadcrumbBar中最后一个item无法点击，需要多加个空元素
    /// </summary>
    public IEnumerable<string> Tags => [..Path.Split('\\'), ""];

    public IEnumerable<TagViewModel> SuggestionTags => Path.TagSuggest('\\', tcb.TagsSource);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFocused))]
    private bool _state = true;

    public bool IsFocused => Path is "" || State;
}
