using CommunityToolkit.Mvvm.ComponentModel;
using TagsTree.Models;

namespace TagsTree.ViewModels;

public sealed partial class TagsManagerViewModel : ObservableObject
{
    public TagsManagerViewModel()
    {
        TagsSource.DeserializeTree(App.TagsPath);
        TagsSource.LoadDictionary();
    }

    public TagsTreeDictionary TagsSource { get; } = new();
    [ObservableProperty] private string _name = "";
}