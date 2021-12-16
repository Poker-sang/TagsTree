using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.ViewModels
{
    public partial class TagEditFilesViewModel : ObservableObject
    {
        public TagViewModel TagViewModel = null!;
        public IEnumerable<string>? Tags => TagViewModel?.FullName.Split("\\");
        [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = Enumerable.Empty<FileViewModel>().ToObservableCollection();
    }
}