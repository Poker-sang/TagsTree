using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.Views.ViewModels;

public partial class TagSearchFilesViewModel : ObservableObject
{
    private ObservableCollection<FileViewModel> _resultCallBack = [];

    [ObservableProperty] private ObservableCollection<FileViewModel> _fileViewModels = [];

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
