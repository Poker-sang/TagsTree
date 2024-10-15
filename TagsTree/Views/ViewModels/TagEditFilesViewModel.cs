using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.Views.ViewModels;

public partial class TagEditFilesViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Tags))]
    private TagViewModel _tagViewModel = null!;

    public IEnumerable<string> Tags => (TagViewModel?.FullName + '\\').Split('\\');

    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = [];
}
