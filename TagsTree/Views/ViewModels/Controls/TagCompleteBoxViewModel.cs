using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TagsTree.Controls;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Views.ViewModels.Controls;

public partial class TagCompleteBoxViewModel : ObservableObject
{
    private readonly TagCompleteBox _tagCompleteBox;

    public TagCompleteBoxViewModel(TagCompleteBox tcb) => _tagCompleteBox = tcb;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Tags))]
    [NotifyPropertyChangedFor(nameof(SuggestionTags))]
    private string _path = "";

    /// <summary>
    /// BreadcrumbBar中最后一个item无法点击，需要多加个空元素
    /// </summary>
    public IEnumerable<string> Tags => Enumerable.Concat(Path.Split('\\'), new[] { "" });

    public IEnumerable<Views.ViewModels.TagViewModel> SuggestionTags => TagViewModelHelper.TagSuggest(Path, '\\', _tagCompleteBox.TagsSource);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFocused))]
    [NotifyPropertyChangedFor(nameof(IsNotFocused))]
    private bool _state = true;

    public bool IsFocused => Path is "" || State;

    public bool IsNotFocused => !IsFocused;
}
