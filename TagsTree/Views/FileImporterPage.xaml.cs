using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagsTree.Interfaces;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

public partial class FileImporterPage : Page, ITypeGetter
{
    public FileImporterPage()
    {
        _vm = new();
        _vm.FileViewModels.CollectionChanged += (_, _) => BDelete.IsEnabled = BSave.IsEnabled = !Importing && _vm.FileViewModels.Count is not 0;
        InitializeComponent();
    }
    public static Type TypeGetter => typeof(FileImporterPage);

    private readonly FileImporterViewModel _vm;

    private bool _importing;
    private bool Importing
    {
        get => _importing;
        set
        {
            if (Equals(value, _importing))
                return;
            _importing = value;
            PbSave.Opacity = value ? 1 : 0;
            // 虽然写上更安全，但一般看不到这些选项：SelectFiles.IsEnabled = SelectFolders.IsEnabled = PathFiles.IsEnabled = PathFolders.IsEnabled = PathBoth.IsEnabled
            BPath.IsEnabled = BSelect.IsEnabled = BAll.IsEnabled = !value;
            BDelete.IsEnabled = BSave.IsEnabled = !value && _vm.FileViewModels.Count is not 0;
        }
    }

    #region 事件处理

    private async void ImportClick(object sender, RoutedEventArgs e)
    {
        Importing = true;
        await Task.Yield();
        var temp = new List<FileViewModel>();
        var dictionary = new Dictionary<string, bool>();

        if (((FrameworkElement)sender).Name is { } mode)
            if (mode is nameof(SelectFiles))
            {
                if (await FileSystemHelper.GetStorageFiles() is { } files and not { Count: 0 })
                    if (FileViewModel.IsValidPath(files[0].Path.GetPath()))
                    {
                        foreach (var fileViewModelModel in _vm.FileViewModels)
                            dictionary[fileViewModelModel.FullName] = true;
                        if (FileViewModel.IsValidPath(files[0].Path.GetPath()))
                            temp.AddRange(files.Where(file => !dictionary.ContainsKey(false + file.Path))
                                .Select(file => new FileViewModel(file.Path)));
                    }
            }
            else if (await FileSystemHelper.GetStorageFolder() is { } folder)
            {
                foreach (var fileViewModelModel in _vm.FileViewModels)
                    dictionary[fileViewModelModel.FullName] = true;
                if (mode is nameof(SelectFolders))
                {
                    if (FileViewModel.IsValidPath(folder.Path.GetPath()))
                        if (!dictionary.ContainsKey(true + folder.Path))
                            temp.Add(new(folder.Path));
                }
                else if (FileViewModel.IsValidPath(folder.Path))
                    switch (mode)
                    {
                        case nameof(PathFiles):
                            temp.AddRange(new DirectoryInfo(folder.Path).GetFiles()
                                .Where(fileInfo => !dictionary.ContainsKey(false + fileInfo.FullName))
                                .Select(fileInfo => new FileViewModel(fileInfo.FullName)));
                            break;
                        case nameof(PathFolders):
                            temp.AddRange(new DirectoryInfo(folder.Path).GetDirectories()
                                .Where(directoryInfo => !dictionary.ContainsKey(true + directoryInfo.FullName))
                                .Select(directoryInfo => new FileViewModel(directoryInfo.FullName)));
                            break;
                        case nameof(PathBoth):
                        {
                            temp.AddRange(new DirectoryInfo(folder.Path).GetFiles()
                                .Where(fileInfo => !dictionary.ContainsKey(false + fileInfo.FullName))
                                .Select(fileInfo => new FileViewModel(fileInfo.FullName)));
                            temp.AddRange(new DirectoryInfo(folder.Path).GetDirectories()
                                .Where(directoryInfo => !dictionary.ContainsKey(true + directoryInfo.FullName))
                                .Select(directoryInfo => new FileViewModel(directoryInfo.FullName)));
                        }

                        break;
                        case nameof(BAll):
                            void RecursiveReadFiles(string folderName)
                            {
                                temp.AddRange(new DirectoryInfo(folderName).GetFiles()
                                    .Where(fileInfo => !dictionary.ContainsKey(false + fileInfo.FullName))
                                    .Select(fileInfo => new FileViewModel(fileInfo.FullName)));
                                foreach (var directoryInfo in new DirectoryInfo(folderName).GetDirectories())
                                    RecursiveReadFiles(directoryInfo.FullName);
                            }

                            RecursiveReadFiles(folder.Path);
                            break;
                    }
            }

        _vm.FileViewModels = temp.ToObservableCollection();
        Importing = false;
    }

    private void DeleteClick(object sender, RoutedEventArgs e) => _vm.FileViewModels.Clear();

    private async void SaveClick(object sender, RoutedEventArgs e)
    {
        await Task.Yield();
        var saved = 0;
        var dictionary = new Dictionary<string, bool>();
        foreach (var fileModel in App.IdFile.Values)
            dictionary[fileModel.FullName] = true;
        PbSave.Opacity = 1;
        foreach (var fileViewModel in _vm.FileViewModels)
            if (!dictionary.ContainsKey(fileViewModel.FullName))
            {
                FileModel.FromFullName(fileViewModel.FullName).AddNew();
                ++saved;
            }

        App.SaveFiles();
        App.SaveRelations();
        PbSave.Opacity = 0;
        IbSave.Message = $"导入「{saved}/{_vm.FileViewModels.Count}」个文件";
        _vm.FileViewModels.Clear();
        IbSave.IsOpen = true;
    }

    private void ContextDeleteClick(object sender, RoutedEventArgs e)
    {
        foreach (var fileViewModel in FileImporterDataGird.SelectedItems.Cast<FileViewModel>().ToList())
            _ = _vm.FileViewModels.Remove(fileViewModel);
    }

    #endregion
}
