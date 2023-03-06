using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TagsTree.Models;

namespace TagsTree.Views.ViewModels;

public partial class FilesObserverViewModel : ObservableObject
{
    public ObservableCollection<FileChanged> FilesChangedList { get; }

    public int FirstId => FilesChangedList.Count is 0 ? 0 : FilesChangedList[0].Id;

    public int LastId => FilesChangedList.Count is 0 ? 1 : FilesChangedList[^1].Id;

    public FilesObserverViewModel(ObservableCollection<FileChanged> filesChangedList)
    {
        FilesChangedList = filesChangedList;
        FilesChangedList.CollectionChanged += (_, e) =>
        {
            if (e.Action is NotifyCollectionChangedAction.Remove)
                FileChanged.Num = FilesChangedList.LastOrDefault() is { } fileChanged ? fileChanged.Id + 1 : 1;
            IsSaveEnabled = true;
            OnPropertyChanged(nameof(FirstId));
            OnPropertyChanged(nameof(LastId));
            OnPropertyChanged(nameof(IsListNotEmpty));
            OnPropertyChanged(nameof(IsMultipleItems));
        };
    }

    [ObservableProperty] private bool _isSaveEnabled;

    public bool IsListNotEmpty => FilesChangedList.Count is not 0;

    public bool IsMultipleItems => FilesChangedList.Count > 1;
}
