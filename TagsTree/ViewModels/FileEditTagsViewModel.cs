using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3Utilities;

namespace TagsTree.ViewModels;

public partial class FileEditTagsViewModel : ObservableObject
{
    public void Load(FileViewModel fileViewModel)
    {
        _fileViewModel = fileViewModel;
        VirtualTags = App.Relations.GetTags(fileViewModel.Id).ToObservableCollection();
    }

    public static ObservableCollection<TagViewModel> TagsSource => App.Tags.TagsTree.SubTags;

    [ObservableProperty] private FileViewModel _fileViewModel = null!;
    [ObservableProperty] private ObservableCollection<TagViewModel> _virtualTags = null!;
}
