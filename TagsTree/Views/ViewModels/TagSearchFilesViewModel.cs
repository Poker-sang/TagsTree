using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3Utilities;

namespace TagsTree.Views.ViewModels;

public partial class TagSearchFilesViewModel : ObservableObject
{
    private ObservableCollection<FileViewModel> _resultCallBack = Enumerable.Empty<FileViewModel>().ToObservableCollection();

    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = Enumerable.Empty<FileViewModel>().ToObservableCollection();

    public ObservableCollection<FileViewModel> ResultCallBack
    {
        get => _resultCallBack;
        set
        {
            if (Equals(_resultCallBack, value))
                return;
            _resultCallBack = value;
            FileViewModels = value;
        }
    }
}
