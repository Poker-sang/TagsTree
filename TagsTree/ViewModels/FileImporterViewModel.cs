using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.ViewModels;

public partial class FileImporterViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = new();
}
