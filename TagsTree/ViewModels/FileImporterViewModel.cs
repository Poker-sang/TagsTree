using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.ViewModels;

public partial class FileImporterViewModel : ObservableObject
{
    public FileImporterViewModel() => FileViewModels.CollectionChanged += (_, _) => OnPropertyChanged(nameof(DeleteSaveEnabled));

    /// <summary>
    /// <see langword="null"/>表示初始未设置的状态，此时<see cref="SavedMessage"/>转<see langword="bool"/>和<see cref="ShowProgressBar"/>都是<see langword="false"/>
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowProgressBar))]
    private string _savedMessage = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowProgressBar))]
    private bool _importing;

    public bool ShowProgressBar => SavedMessage is "" || Importing;

    public bool Imported => !Importing;

    public bool DeleteSaveEnabled => !Importing && FileViewModels.Count is not 0;

    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = new();
}
