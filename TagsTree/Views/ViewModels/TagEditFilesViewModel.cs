using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3Utilities;

namespace TagsTree.Views.ViewModels;

public partial class TagEditFilesViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Tags))]
    private TagViewModel _tagViewModel = null!;

    public IEnumerable<string> Tags => (TagViewModel?.FullName + '\\').Split('\\');

    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = Enumerable.Empty<FileViewModel>().ToObservableCollection();
}
