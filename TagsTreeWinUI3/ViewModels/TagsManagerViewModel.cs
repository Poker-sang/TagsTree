using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels
{
    public sealed partial class TagsManagerViewModel : ObservableObject
    {
        public TagsManagerViewModel()
        {
            TagsSource.DeserializeTree(App.TagsPath);
            TagsSource.LoadDictionary();
        }

        public TreeDictionary TagsSource { get; } = new();
        [ObservableProperty] private string _name = "";
    }
}
