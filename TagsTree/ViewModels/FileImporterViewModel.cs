using System.Collections.ObjectModel;

namespace TagsTree.ViewModels;

public class FileImporterViewModel
{
    public ObservableCollection<FileViewModel> FileViewModels { get; } = new();

}