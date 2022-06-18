using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TagsTree.ViewModels;

public partial class FileImporterViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = new();
}
