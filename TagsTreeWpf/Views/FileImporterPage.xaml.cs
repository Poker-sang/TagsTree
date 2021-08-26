using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TagsTreeWpf.Services;
using TagsTreeWpf.ViewModels;
using static TagsTreeWpf.Properties.Settings;

namespace TagsTreeWpf.Views
{
	/// <summary>
	/// FileImporterPage.xaml 的交互逻辑
	/// </summary>
	public partial class FileImporterPage : Page
	{
		public FileImporterPage()
		{
			DataContext = _vm = new FileImporterViewModel(this);
			InitializeComponent();
		}

		private readonly FileImporterViewModel _vm;

		public async void Import(object? parameter)
		{
			_vm.Importing = true;
			var dialog = new CommonOpenFileDialog
			{
				Multiselect = true,
				EnsurePathExists = true,
				IsFolderPicker = true,
				InitialDirectory = Default.LibraryPath
			};
			switch ((string)parameter!)
			{
				case "Select_Files":
					dialog.Title = "选择你需要引入的文件";
					dialog.IsFolderPicker = false;
					break;
				case "Select_Folders": dialog.Title = "选择你需要引入的文件夹"; break;
				case "Path_Files": dialog.Title = "选择你需要引入的文件所在的文件夹"; break;
				case "Path_Folders": dialog.Title = "选择你需要引入的文件夹所在的文件夹"; break;
				case "Path_Both": dialog.Title = "选择你需要引入的文件和文件夹所在的文件夹"; break;
				case "All": dialog.Title = "选择你需要引入的文件所在的根文件夹"; break;
			}

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
				await Task.Run(() =>
				{
					var dictionary = new Dictionary<string, bool>();
					foreach (var fileViewModelModel in _vm.FileViewModels)
						dictionary[fileViewModelModel.UniqueName] = true;
					switch ((string)parameter!)
					{
						case "Select_Files":
							if (FileViewModel.ValidPath(
								dialog.FileNames.First()[..dialog.FileNames.First().LastIndexOf('\\')]))
								foreach (var fileName in dialog.FileNames)
									if (!dictionary.ContainsKey(false + fileName))
										Application.Current.Dispatcher.Invoke(() =>
											_vm.FileViewModels.Add(new FileViewModel(fileName, false)));
							break;
						case "Select_Folders":
							if (FileViewModel.ValidPath(
								dialog.FileNames.First()[..dialog.FileNames.First().LastIndexOf('\\')]))
								foreach (var directoryName in dialog.FileNames)
									if (!dictionary.ContainsKey(true + directoryName))
										Application.Current.Dispatcher.Invoke(() =>
											_vm.FileViewModels.Add(new FileViewModel(directoryName, true)));
							break;
						case "Path_Files":
							if (FileViewModel.ValidPath(dialog.FileNames.First()))
								foreach (var directoryName in dialog.FileNames)
									foreach (var fileInfo in new DirectoryInfo(directoryName).GetFiles())
										if (!dictionary.ContainsKey(false + fileInfo.FullName))
											Application.Current.Dispatcher.Invoke(() =>
												_vm.FileViewModels.Add(new FileViewModel(fileInfo.FullName, false)));
							break;
						case "Path_Folders":
							if (FileViewModel.ValidPath(dialog.FileNames.First()))
								foreach (var directoryName in dialog.FileNames)
									foreach (var directoryInfo in new DirectoryInfo(directoryName).GetDirectories())
										if (!dictionary.ContainsKey(true + directoryInfo.FullName))
											Application.Current.Dispatcher.Invoke(() =>
												_vm.FileViewModels.Add(new FileViewModel(directoryInfo.FullName, true)));
							break;
						case "Path_Both":
							if (FileViewModel.ValidPath(dialog.FileNames.First()))
								foreach (var directoryName in dialog.FileNames)
								{
									foreach (var fileInfo in new DirectoryInfo(directoryName).GetFiles())
										if (!dictionary.ContainsKey(false + fileInfo.FullName))
											Application.Current.Dispatcher.Invoke(() =>
												_vm.FileViewModels.Add(new FileViewModel(fileInfo.FullName, false)));
									foreach (var directoryInfo in new DirectoryInfo(directoryName).GetDirectories())
										if (!dictionary.ContainsKey(true + directoryInfo.FullName))
											Application.Current.Dispatcher.Invoke(() =>
												_vm.FileViewModels.Add(new FileViewModel(directoryInfo.FullName,
													true)));
								}

							break;
						case "All":

							void RecursiveReadFiles(string folderName)
							{
								foreach (var fileInfo in new DirectoryInfo(folderName).GetFiles())
									if (!dictionary!.ContainsKey(false + fileInfo.FullName))
										Application.Current.Dispatcher.Invoke(() =>
											_vm.FileViewModels.Add(new FileViewModel(fileInfo.FullName, false)));
								foreach (var directoryInfo in new DirectoryInfo(folderName).GetDirectories())
									RecursiveReadFiles(directoryInfo.FullName);
							}

							if (FileViewModel.ValidPath(dialog.FileNames.First()))
								foreach (var directoryName in dialog.FileNames)
									RecursiveReadFiles(directoryName);
							break;
					}
				});
			_vm.Importing = false;
		}

		public void DeleteBClick(object? parameter) => _vm.FileViewModels.Clear();

		public async void SaveBClick(object? parameter)
		{
			var progressBar = new ProcessBarHelper(FileImporterGrid);
			var duplicated = 0;
			await Task.Run(() =>
			{
				var dictionary = new Dictionary<string, bool>();
				foreach (var fileModel in App.IdFile.Values)
					dictionary[fileModel.UniqueName] = true;
				progressBar.ProcessValue = 1;
				var unit = 98.0 / _vm.FileViewModels.Count;
				foreach (var fileModel in _vm.FileViewModels)
				{
					if (!dictionary.ContainsKey(fileModel.UniqueName))
					{
						App.Relations.NewRow(fileModel.NewFileModel());
						App.IdFile[fileModel.Id] = fileModel.NewFileModel();
					}
					else duplicated++;

					progressBar.ProcessValue += unit;
				}
			});
			App.SaveFiles();
			App.SaveRelations();
			progressBar.ProcessValue = 100;
			var former = _vm.FileViewModels.Count;
			_vm.FileViewModels.Clear();
			progressBar.Dispose();
			MessageBoxX.Information($"共导入「{former}」个文件，其中成功导入「{former - duplicated}」个，有「{duplicated}」个因重复未导入");
		}
	}
}

