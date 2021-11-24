using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TagsTree.ViewModels
{
    public sealed partial class TagsManagerViewModel : ObservableObject
    {
        public ObservableCollection<TagViewModel> TagsSource { get; set; } = App.Tags.TagsTree.SubTags;
        [ObservableProperty] private string _name = "";
    }
}
