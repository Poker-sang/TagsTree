using CommunityToolkit.Mvvm.ComponentModel;
using TagsTree.Models;

namespace TagsTree.ViewModels;

public sealed partial class TagsManagerViewModel : ObservableObject
{
    public TagsManagerViewModel()
    {
        TagsSource.DeserializeTree(AppContext.TagsPath);
        TagsSource.LoadDictionary();
    }

    public TagsTreeDictionary TagsSource { get; } = new();

    [ObservableProperty] private string _name = "";

    [ObservableProperty] private string _path = "";

    public bool CanPaste => ClipBoard is not null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPaste))]
    private TagViewModel? _clipBoard;

    [ObservableProperty] private bool _isSaveEnabled;
}
