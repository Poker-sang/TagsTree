using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels
{
    public partial class TagEditFilesViewModel : ObservableObject
    {
        public ObservableCollection<TagViewModel> TagsSource { get; set; } = App.Tags.TagsTree.SubTags;
        [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = Enumerable.Empty<FileViewModel>().ToObservableCollection();
    }
}