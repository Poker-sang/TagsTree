using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.ViewModels
{
    public class TagEditFilesViewModel : ObservableObject
    {
        public ObservableCollection<TagViewModel> TagsSource { get; set; } = App.Tags.TagsTree.SubTags;

        private ObservableCollection<FileViewModel> _fileViewModels = Enumerable.Empty<FileViewModel>().ToObservableCollection();

        public ObservableCollection<FileViewModel> FileViewModels
        {
            get => _fileViewModels;
            set
            {
                if (Equals(_fileViewModels, value)) return;
                _fileViewModels = value;
                OnPropertyChanged(nameof(FileViewModels));
            }
        }
    }
}