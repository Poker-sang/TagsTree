using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.Views.ViewModels;

public partial class FileImporterViewModel : ObservableObject
{
    public FileImporterViewModel() => FileViewModels.CollectionChanged += (_, _) => OnPropertyChanged(nameof(DeleteSaveEnabled));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Processed))]
    [NotifyPropertyChangedFor(nameof(DeleteSaveEnabled))]
    private bool _processing;

    public bool Processed => !Processing;

    public bool DeleteSaveEnabled => !Processing && FileViewModels.Count is not 0;

    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = [];
}
