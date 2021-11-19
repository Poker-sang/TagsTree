using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Views
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
            await Task.Yield();
            if ((string)parameter! is "Select_Files")
            {
                if (await FileSystemHelper.GetStorageFiles() is { } files and not { Count: 0 })
                {
                    var dictionary = new Dictionary<string, bool>();
                    foreach (var fileViewModelModel in _vm.FileViewModels)
                        dictionary[fileViewModelModel.UniqueName] = true;
                    if (FileViewModel.ValidPath(files[0].Path.GetPath()))
                        foreach (var file in files)
                            if (!dictionary.ContainsKey(false + file.Path))
                                _ = DataGrid.DispatcherQueue.TryEnqueue(() => _vm.FileViewModels.Add(new FileViewModel(file.Path)));
                }
                _vm.Importing = false;
                return;
            }

            if (await FileSystemHelper.GetStorageFolder() is { } folder)
            {
                var dictionary = new Dictionary<string, bool>();
                foreach (var fileViewModelModel in _vm.FileViewModels)
                    dictionary[fileViewModelModel.UniqueName] = true;
                switch ((string)parameter!)
                {
                    case "Select_Folders":
                        if (FileViewModel.ValidPath(folder.Path.GetPath()))
                            if (!dictionary.ContainsKey(true + folder.Path))
                                _ = DataGrid.DispatcherQueue.TryEnqueue(() => _vm.FileViewModels.Add(new FileViewModel(folder.Path)));
                        break;
                    case "Path_Files":
                        if (FileViewModel.ValidPath(folder.Path))
                            foreach (var fileInfo in new DirectoryInfo(folder.Path).GetFiles())
                                if (!dictionary.ContainsKey(false + fileInfo.FullName))
                                    _ = DataGrid.DispatcherQueue.TryEnqueue(() => _vm.FileViewModels.Add(new FileViewModel(fileInfo.FullName)));
                        break;
                    case "Path_Folders":
                        if (FileViewModel.ValidPath(folder.Path))
                            foreach (var directoryInfo in new DirectoryInfo(folder.Path).GetDirectories())
                                if (!dictionary.ContainsKey(true + directoryInfo.FullName))
                                    _ = DataGrid.DispatcherQueue.TryEnqueue(() => _vm.FileViewModels.Add(new FileViewModel(directoryInfo.FullName)));
                        break;
                    case "Path_Both":
                        if (FileViewModel.ValidPath(folder.Path))
                        {
                            foreach (var fileInfo in new DirectoryInfo(folder.Path).GetFiles())
                                if (!dictionary.ContainsKey(false + fileInfo.FullName))
                                    _ = DataGrid.DispatcherQueue.TryEnqueue(() => _vm.FileViewModels.Add(new FileViewModel(fileInfo.FullName)));
                            foreach (var directoryInfo in new DirectoryInfo(folder.Path).GetDirectories())
                                if (!dictionary.ContainsKey(true + directoryInfo.FullName))
                                    _ = DataGrid.DispatcherQueue.TryEnqueue(() => _vm.FileViewModels.Add(new FileViewModel(directoryInfo.FullName)));
                        }
                        break;
                    case "All":
                        void RecursiveReadFiles(string folderName)
                        {
                            foreach (var fileInfo in new DirectoryInfo(folderName).GetFiles())
                                if (!dictionary!.ContainsKey(false + fileInfo.FullName))
                                    _ = DataGrid.DispatcherQueue.TryEnqueue(() => _vm.FileViewModels.Add(new FileViewModel(fileInfo.FullName)));
                            foreach (var directoryInfo in new DirectoryInfo(folderName).GetDirectories())
                                RecursiveReadFiles(directoryInfo.FullName);
                        }
                        if (FileViewModel.ValidPath(folder.Path))
                            RecursiveReadFiles(folder.Path);
                        break;
                }
            }

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
            await MessageDialogX.Information(false, $"共导入「{former}」个文件，其中成功导入「{former - duplicated}」个，有「{duplicated}」个因重复未导入");
        }
    }
}

