using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.Views.ViewModels;

public partial class SelectTagToEditPageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<TagViewModel> _allTags = AppContext.Tags.TagsTree.SubTags;

    [ObservableProperty] private string _path = "";
}
