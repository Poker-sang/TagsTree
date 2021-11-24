using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TagsTree.Commands;
using TagsTree.Views;

namespace TagsTree.ViewModels
{
    public partial class FileImporterViewModel : ObservableObject
    {
        public FileImporterViewModel(FileImporterPage page)
        {
            Import = new RelayCommand(_ => !Importing, page.Import);
            DeleteBClick = new RelayCommand(_ => !Importing && FileViewModels.Count != 0, page.DeleteBClick);
            SaveBClick = new RelayCommand(_ => !Importing && FileViewModels.Count != 0, page.SaveBClick);

            FileViewModels.CollectionChanged += (_, _) =>
            {
                DeleteBClick.OnCanExecuteChanged();
                SaveBClick.OnCanExecuteChanged();
            };
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is nameof(Importing))
                {
                    Import.OnCanExecuteChanged();
                    DeleteBClick.OnCanExecuteChanged();
                    SaveBClick.OnCanExecuteChanged();
                }
            };
        }
        public RelayCommand Import { get; }
        public RelayCommand DeleteBClick { get; }
        public RelayCommand SaveBClick { get; }
        public ObservableCollection<FileViewModel> FileViewModels { get; } = new();

        [ObservableProperty] private bool _importing;
    }
}
