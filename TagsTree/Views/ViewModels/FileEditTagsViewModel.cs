using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.Views.ViewModels;

public partial class FileEditTagsViewModel(FileViewModel fileViewModel) : ObservableObject
{
    public FileEditTagsViewModel() : this(FileViewModel.DefaultFileViewModel)
    {
    }

    [ObservableProperty] private bool _isSaveEnabled;

    public static ObservableCollection<TagViewModel> TagsSource => AppContext.Tags.TagsTree.SubTags;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VirtualTags))]
    private FileViewModel _fileViewModel = fileViewModel;

    public ObservableCollection<TagViewModel> VirtualTags { get; } = [..AppContext.Relations.GetTags(fileViewModel.Id)];
}
