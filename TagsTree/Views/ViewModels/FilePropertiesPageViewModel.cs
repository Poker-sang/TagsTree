using CommunityToolkit.Mvvm.ComponentModel;

namespace TagsTree.Views.ViewModels;

public partial class FilePropertiesPageViewModel : ObservableObject
{
    [ObservableProperty] private FileViewModel _fileViewModel = FileViewModel.DefaultFileViewModel;

    public void RaiseOnPropertyChanged() => OnPropertyChanged(nameof(FileViewModel));
}
