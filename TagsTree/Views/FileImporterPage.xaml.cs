using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

/// <summary>
/// FileImporterPage.xaml 的交互逻辑
/// </summary>
public partial class FileImporterPage : Page
{
    public FileImporterPage()
    {
        _vm = new FileImporterViewModel();
        _vm.FileViewModels.CollectionChanged += (_, _) => BDelete.IsEnabled = BSave.IsEnabled = !Importing && _vm.FileViewModels.Count is not 0;
        InitializeComponent();
    }

    private readonly FileImporterViewModel _vm;

    private bool _importing;
    private bool Importing
    {
        get => _importing;
        set
        {
            if (Equals(value, _importing)) return;
            _importing = value;
            BPath.IsEnabled = BSelect.IsEnabled = !value; //虽然写上更安全，但一般看不到这些选项：SelectFiles.IsEnabled = SelectFolders.IsEnabled = PathFiles.IsEnabled = PathFolders.IsEnabled = PathBoth.IsEnabled = All.IsEnabled
            BDelete.IsEnabled = BSave.IsEnabled = !value && _vm.FileViewModels.Count is not 0;
        }
    }

    private async void Import(object sender, RoutedEventArgs e)
    {
        Importing = true;
        var temp = new List<FileViewModel>();
        var dictionary = new Dictionary<string, bool>();

        if (((FrameworkElement)sender).Name is { } mode)
            if (mode is nameof(SelectFiles))
            {
                if (await FileSystemHelper.GetStorageFiles() is { } files and not { Count: 0 })
                    await Task.Run(() =>
                    {
                        if (FileViewModel.IsValidPath(files[0].Path.GetPath()))
                        {
                            foreach (var fileViewModelModel in _vm.FileViewModels)
                                dictionary[fileViewModelModel.UniqueName] = true;
                            if (FileViewModel.IsValidPath(files[0].Path.GetPath()))
                                temp.AddRange(files.Where(file => !dictionary.ContainsKey(false + file.Path))
                                    .Select(file => new FileViewModel(file.Path)));
                        }
                    });
            }
            else if (await FileSystemHelper.GetStorageFolder() is { } folder)
                await Task.Run(() =>
                {
                    foreach (var fileViewModelModel in _vm.FileViewModels)
                        dictionary[fileViewModelModel.UniqueName] = true;
                    if (mode is nameof(SelectFolders))
                    {
                        if (FileViewModel.IsValidPath(folder.Path.GetPath()))
                            if (!dictionary.ContainsKey(true + folder.Path))
                                temp.Add(new FileViewModel(folder.Path));
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
                });
        foreach (var fileViewModel in temp)
            _vm.FileViewModels.Add(fileViewModel);
        Importing = false;
    }

    private void DeleteBClick(object sender, RoutedEventArgs e) => _vm.FileViewModels.Clear();

    private async void SaveBClick(object sender, RoutedEventArgs e)
    {
        var progressBar = new ProcessBarHelper((Grid)((FrameworkElement)sender).Tag);
        var duplicated = 0;
        await Task.Yield();
        var dictionary = new Dictionary<string, bool>();
        foreach (var fileModel in App.IdFile.Values)
            dictionary[fileModel.UniqueName] = true;
        progressBar.ProcessValue = 1;
        var unit = 98.0 / _vm.FileViewModels.Count;
        foreach (var fileViewModel in _vm.FileViewModels)
        {
            if (!dictionary.ContainsKey(fileViewModel.UniqueName))
                fileViewModel.AddNew();
            else duplicated++;
            progressBar.ProcessValue += unit;
        }
        App.SaveFiles();
        App.SaveRelations();
        progressBar.ProcessValue = 100;
        var former = _vm.FileViewModels.Count;
        _vm.FileViewModels.Clear();
        progressBar.Dispose();
        await ShowMessageDialog.Information(false, $"共导入「{former}」个文件，其中成功导入「{former - duplicated}」个，有「{duplicated}」个因重复未导入");
    }

    private void DeleteCmClick(object sender, RoutedEventArgs e)
    {
        foreach (var fileViewModel in FileImporterDataGird.SelectedItems.Cast<FileViewModel>().ToList())
            _ = _vm.FileViewModels.Remove(fileViewModel);
    }
}