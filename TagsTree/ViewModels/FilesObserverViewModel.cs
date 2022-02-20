using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TagsTree.Models;

namespace TagsTree.ViewModels;

public class FilesObserverViewModel : ObservableObject
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
                if (FilesChangedList.LastOrDefault() is { } fileChanged)
                    FileChanged.Num = fileChanged.Id + 1;
                else FileChanged.Num = 1;
            OnPropertyChanged(nameof(FirstId));
            OnPropertyChanged(nameof(LastId));
        };
    }
}