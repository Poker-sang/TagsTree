using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.Views.ViewModels;

public partial class FileEditTagsViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSaveEnabled;

    public static ObservableCollection<TagViewModel> TagsSource => AppContext.Tags.TagsTree.SubTags;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VirtualTags))]
    private FileViewModel _fileViewModel = FileViewModel.DefaultFileViewModel;

    public ObservableCollection<TagViewModel> VirtualTags => [..AppContext.Relations.GetTags(FileViewModel.Id)];
}
