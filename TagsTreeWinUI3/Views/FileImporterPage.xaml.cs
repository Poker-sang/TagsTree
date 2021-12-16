using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views
{
    /// <summary>
    /// FileImporterPage.xaml 的交互逻辑
    /// </summary>
    public partial class FileImporterPage : Page
    {
        public FileImporterPage()
        {
            _vm = new FileImporterViewModel(this);
            InitializeComponent();
        }

        private readonly FileImporterViewModel _vm;

        public async void Import(object? parameter)
        {
            _vm.Importing = true;
            var temp = new List<FileViewModel>();
            if ((string)parameter! is "Select_Files")
            {
                if (await FileSystemHelper.GetStorageFiles() is { } files and not { Count: 0 })
                {
                    await Task.Run(() =>
                    {
                        var dictionary = new Dictionary<string, bool>();
                        foreach (var fileViewModelModel in _vm.FileViewModels)
                            dictionary[fileViewModelModel.UniqueName] = true;
                        if (FileViewModel.IsValidPath(files[0].Path.GetPath()))
                            temp.AddRange(files.Where(file => !dictionary.ContainsKey(false + file.Path))
                                .Select(file => new FileViewModel(file.Path)));
                    });
                }
            }
            else if (await FileSystemHelper.GetStorageFolder() is { } folder)
            {
                await Task.Run(() =>
                {
                    var dictionary = new Dictionary<string, bool>();
                    foreach (var fileViewModelModel in _vm.FileViewModels)
                        dictionary[fileViewModelModel.UniqueName] = true;
                    switch ((string)parameter!)
                    {
                        case "Select_Folders":
                            if (FileViewModel.IsValidPath(folder.Path.GetPath()) &&
                                !dictionary.ContainsKey(true + folder.Path))
                                temp.Add(new FileViewModel(folder.Path));
                            break;
                        case "Path_Files":
                            if (FileViewModel.IsValidPath(folder.Path))
                                temp.AddRange(new DirectoryInfo(folder.Path).GetFiles()
                                    .Where(fileInfo => !dictionary.ContainsKey(false + fileInfo.FullName))
                                    .Select(fileInfo => new FileViewModel(fileInfo.FullName)));
                            break;
                        case "Path_Folders":
                            if (FileViewModel.IsValidPath(folder.Path))
                                temp.AddRange(new DirectoryInfo(folder.Path).GetDirectories()
                                    .Where(directoryInfo => !dictionary.ContainsKey(true + directoryInfo.FullName))
                                    .Select(directoryInfo => new FileViewModel(directoryInfo.FullName)));
                            break;
                        case "Path_Both":
                            if (FileViewModel.IsValidPath(folder.Path))
                            {
                                temp.AddRange(new DirectoryInfo(folder.Path).GetFiles()
                                    .Where(fileInfo => !dictionary.ContainsKey(false + fileInfo.FullName))
                                    .Select(fileInfo => new FileViewModel(fileInfo.FullName)));
                                temp.AddRange(new DirectoryInfo(folder.Path).GetDirectories()
                                    .Where(directoryInfo => !dictionary.ContainsKey(true + directoryInfo.FullName))
                                    .Select(directoryInfo => new FileViewModel(directoryInfo.FullName)));
                            }
                            break;
                        case "All":
                            void RecursiveReadFiles(string folderName)
                            {
                                temp.AddRange(new DirectoryInfo(folderName).GetFiles()
                                    .Where(fileInfo => !dictionary!.ContainsKey(false + fileInfo.FullName))
                                    .Select(fileInfo => new FileViewModel(fileInfo.FullName)));
                                foreach (var directoryInfo in new DirectoryInfo(folderName).GetDirectories())
                                    RecursiveReadFiles(directoryInfo.FullName);
                            }
                            if (FileViewModel.IsValidPath(folder.Path))
                                RecursiveReadFiles(folder.Path);
                            break;
                    }
                });
            }
            foreach (var fileViewModel in temp)
                _vm.FileViewModels.Add(fileViewModel);
            _vm.Importing = false;
        }

        public void DeleteBClick(object? parameter) => _vm.FileViewModels.Clear();

        public async void SaveBClick(object? parameter)
        {
            var progressBar = new ProcessBarHelper((Grid)parameter!);
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
}

