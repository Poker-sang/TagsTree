using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using TagsTree.Interfaces;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class FileImporterPage : Page, ITypeGetter
{
    public FileImporterPage() => InitializeComponent();

    public static Type TypeGetter => typeof(FileImporterPage);

    private readonly FileImporterViewModel _vm = new();

    #region 事件处理

    private async void ImportClicked(object sender, RoutedEventArgs e)
    {
        _vm.Processing = true;
        await Task.Yield();
        var temp = new List<FileViewModel>();
        var dictionary = new Dictionary<string, bool>();

        if (sender.To<FrameworkElement>().Name is { } mode)
            if (mode is nameof(SelectFiles))
            {
                if (await App.MainWindow.PickMultipleFilesAsync() is { } files and not { Count: 0 })
                    if (FileViewModel.IsValidPath(files[0].Path.GetPath()))
                    {
                        foreach (var fileViewModelModel in _vm.FileViewModels)
                            dictionary[fileViewModelModel.FullName] = true;
                        if (FileViewModel.IsValidPath(files[0].Path.GetPath()))
                            temp.AddRange(files.Where(file => !dictionary.ContainsKey(false + file.Path))
                                .Select(file => new FileViewModel(file.Path)));
                    }
            }
            else if (await App.MainWindow.PickSingleFolderAsync() is { } folder)
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
                        case nameof(All):
                            void RecursiveReadFiles(string folderName)
                            {
                                temp.AddRange(new DirectoryInfo(folderName).GetFiles()
                                    .Where(fileInfo => !dictionary!.ContainsKey(false + fileInfo.FullName))
                                    .Select(fileInfo => new FileViewModel(fileInfo.FullName)));
                                foreach (var directoryInfo in new DirectoryInfo(folderName).GetDirectories())
                                    RecursiveReadFiles(directoryInfo.FullName);
                            }

                            RecursiveReadFiles(folder.Path);
                            break;
                    }
            }

        _vm.FileViewModels = [..temp];
        _vm.Processing = false;
    }

    private void DeleteClicked(object sender, RoutedEventArgs e) => _vm.FileViewModels.Clear();

    private void SaveClicked(object sender, RoutedEventArgs e)
    {
        var saved = 0;
        _vm.Processing = true;

        var dictionary = new Dictionary<string, bool>();
        foreach (var fileModel in AppContext.IdFile.Values)
            dictionary[fileModel.FullName] = true;
        foreach (var fileViewModel in _vm.FileViewModels)
            if (!dictionary.ContainsKey(fileViewModel.FullName))
            {
                FileModel.FromFullName(fileViewModel.FullName).AddNew();
                ++saved;
            }

        _vm.Processing = false;

        AppContext.SaveFiles();
        AppContext.SaveRelations();
        this.CreateTeachingTip().ShowAndHide($"导入「{saved}/{_vm.FileViewModels.Count}」个文件");
        _vm.FileViewModels.Clear();
    }

    private void ContextDeleteClicked(object sender, RoutedEventArgs e)
    {
        foreach (var fileViewModel in FileImporterDataGird.SelectedItems.Cast<FileViewModel>())
            _ = _vm.FileViewModels.Remove(fileViewModel);
    }

    #endregion
}
